using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using JetBrains.Annotations;
using NeoCore.Assets;
using NeoCore.CoreClr.Meta;
using NeoCore.Import.Attributes;
using NeoCore.Interop;
using NeoCore.Memory;
using NeoCore.Model;
using NeoCore.Utilities;
using NeoCore.Utilities.Diagnostics;

// ReSharper disable ParameterTypeCanBeEnumerable.Global

namespace NeoCore.Import
{
	
	public sealed partial class ImportManager : Releasable
	{
		#region Constants

		private const string GET_PROPERTY_PREFIX      = "get_";
		private const string GET_PROPERTY_REPLACEMENT = "Get";

		protected override string Id => nameof(ImportManager);

		private delegate void LoadFunction(ImportAttribute attr, MethodInfo memberInfo, Pointer<byte> ptr);

		#endregion

		#region Singleton

		/// <summary>
		///     Gets an instance of <see cref="ImportManager" />
		/// </summary>
		public static ImportManager Value { get; private set; } = new ImportManager();

		private ImportManager()
		{
			Setup();
		}

		#endregion

		#region Override

		public override void Close()
		{
			UnloadAll();

			// Sanity check
			CloseCheck();

			// Delete instance
			Value = null;

			base.Close();
		}

		#endregion

		#region Fields

		private readonly ISet<Type> m_boundTypes = new HashSet<Type>();

		private readonly Dictionary<Type, ImportMap> m_typeImportMaps = new Dictionary<Type, ImportMap>();

		#endregion


		private static string ResolveIdentifier(ImportAttribute      attr,
		                                        [NotNull] MemberInfo member,
		                                        out       string     resolvedId)
		{
			Guard.AssertNotNull(member.DeclaringType, nameof(member.DeclaringType));
			
			CheckAnnotations(member, true, out var nameSpaceAttr);
			
			// Resolve the symbol

			resolvedId = attr.Identifier ?? member.Name;

			string nameSpace          = nameSpaceAttr.Namespace;
			string enclosingNamespace = member.DeclaringType.Name;

			var options = attr.Options;

			bool isMethod   = member.MemberType == MemberTypes.Method;
			bool isCallAttr = attr is ImportCallAttribute;

			var  callAttr = (ImportCallAttribute) attr;
			bool isCtor   = callAttr.CallOptions.HasFlagFast(ImportCallOptions.Constructor);

			if (isMethod && isCallAttr && isCtor) {
				CheckConstructorOptions(options);

				return Format.Combine(enclosingNamespace, enclosingNamespace);
			}


			if (!options.HasFlagFast(IdentifierOptions.IgnoreEnclosingNamespace)) {
				resolvedId = Format.Combine(enclosingNamespace, resolvedId);
			}

			if (!options.HasFlagFast(IdentifierOptions.IgnoreNamespace)) {
				if (nameSpace != null) {
					resolvedId = Format.Combine(nameSpace, resolvedId);
				}
			}

			if (options.HasFlagFast(IdentifierOptions.UseAccessorName)) {
				Guard.Assert(member.MemberType == MemberTypes.Method);
				resolvedId = resolvedId.Replace(GET_PROPERTY_PREFIX, GET_PROPERTY_REPLACEMENT);
			}

			Guard.AssertNotNull(resolvedId, nameof(resolvedId));

			return resolvedId;
		}


		#region Unload

		private void UnloadMap(Type type, FieldInfo mapField)
		{
			var map = m_typeImportMaps[type];

			map.Clear();

			m_typeImportMaps.Remove(type);

			mapField.SetValue(null, null);

			// Sanity check
			CheckMapFieldUnload(type, mapField);
		}

		/// <summary>
		/// Root unload function. Unloads and restores the type <paramref name="type"/>.
		/// </summary>
		public void Unload(Type type)
		{
			if (!IsBound(type)) {
				return;
			}

			Global.Value.WriteInformation(null, "Unloading {name}", type.Name);

			if (UsingMap(type, out var mapField)) {
				UnloadMap(type, mapField);

				Global.Value.WriteVerbose("Unloaded map in {Name}", type.Name);
			}


			(MemberInfo[] members, ImportAttribute[] attributes) = type.GetAnnotated<ImportAttribute>();

			int lim = attributes.Length;

			for (int i = 0; i < lim; i++) {
				var mem  = members[i];
				var attr = attributes[i];

				bool wasBound = attr is ImportCallAttribute callAttr &&
				                callAttr.CallOptions.HasFlagFast(ImportCallOptions.Bind);

				switch (mem.MemberType) {
					case MemberTypes.Property:
						var propInfo = (PropertyInfo) mem;
						var get      = propInfo.GetMethod;

						// Calling the function will now result in an access violation
						if (wasBound) {
							FunctionFactory.Managed.Restore(get);
						}

						break;
					case MemberTypes.Field:
						// The field will be deleted later
						var fi = (FieldInfo) mem;
						if (fi.IsStatic) {
							fi.SetValue(null, default);
						}

						break;
					case MemberTypes.Method:

						// Calling the function will now result in an access violation
						if (wasBound) {
							FunctionFactory.Managed.Restore((MethodInfo) mem);
						}

						break;
					default:
						throw new ArgumentOutOfRangeException();
				}

				Global.Value.WriteVerbose("Unloaded member {Name}", mem.Name);
			}

			m_boundTypes.Remove(type);

			Global.Value.WriteVerbose(Id, "Unloaded {Name}", type.Name);
		}

		public void Unload<T>(ref T value)
		{
			var type = value.GetType();

			Unload(type);

			Mem.Delete(ref value);
		}

		public void UnloadAll(Type[] t)
		{
			foreach (var type in t) {
				Unload(type);
			}
		}

		public void UnloadAll() => UnloadAll(m_boundTypes.ToArray());

		#endregion

		#region Load

		#region Map

		private void AddMapToDictionary(Type t, FieldInfo field)
		{
			if (m_typeImportMaps.ContainsKey(t)) {
				return;
			}

			CheckImportMap(field);

			var map = (ImportMap) field.GetValue(null);
			m_typeImportMaps.Add(t, map);
		}


		private static FieldInfo FindMapField(Type type)
		{
			var mapField = type.GetAnyField(ImportMap.FIELD_NAME);

			CheckImportMapAnnotation(mapField);

			if (mapField == null) {
				var (member, _) = type.GetFirstAnnotated<ImportMapFieldAttribute>();

				if (member != null) {
					mapField = (FieldInfo) member;
				}
			}

			return mapField;
		}

		private static bool UsingMap(Type type, out FieldInfo mapField)
		{
			mapField = FindMapField(type);

			bool exists = mapField != null;

			if (exists) {
				CheckImportMap(mapField);
			}

			return exists;
		}

		#endregion

		/// <summary>
		///     Root load function. Loads <paramref name="value" /> of type <paramref name="type" /> using the
		///     specified <see cref="IImportProvider" /> <paramref name="ip" />.
		/// </summary>
		/// <param name="value">Value of type <paramref name="type" /> to load</param>
		/// <param name="type"><see cref="MetaType" /> of <paramref name="value" /></param>
		/// <param name="ip"><see cref="IImportProvider" /> to use to load components</param>
		/// <typeparam name="T">Type of <paramref name="value" /></typeparam>
		/// <returns><paramref name="value" />, fully loaded</returns>
		private T Load<T>(T value, Type type, IImportProvider ip)
		{
			if (IsBound(type)) {
				return value;
			}
			
			CheckAnnotations(type, false, out _);

			if (UsingMap(type, out var mapField)) {
				if (m_typeImportMaps.ContainsKey(type)) {
					return value;
				}

				AddMapToDictionary(type, mapField);
				value = LoadComponents(value, type, ip, LoadMethod);
			}
			else {
				value = LoadComponents(value, type, ip);
			}

			m_boundTypes.Add(type);


			Global.Value.WriteInformation(null, "Loaded {Name}", type.Name);
			return value;
		}

		/// <summary>
		///     Loads <paramref name="value" /> using <paramref name="ip" />.
		/// </summary>
		/// <param name="value">Value to load</param>
		/// <param name="ip"><see cref="IImportProvider" /> to use</param>
		/// <typeparam name="T">Type of <paramref name="value" /></typeparam>
		/// <returns><paramref name="value" />, fully loaded</returns>
		public T Load<T>(T value, IImportProvider ip) => Load(value, value.GetType(), ip);

		/// <summary>
		///     Loads any non-instance components of type <paramref name="t" />.
		/// </summary>
		/// <param name="t"><see cref="Type" /> to load</param>
		/// <param name="ip"><see cref="IImportProvider" /> to use</param>
		/// <returns>A <c>default</c> object of type <paramref name="t" /></returns>
		public object Load(Type t, IImportProvider ip) => Load(default(object), t, ip);

		public void LoadAll(Type[] t, IImportProvider ip)
		{
			foreach (var type in t) {
				Load(type, ip);
			}
		}


		private void LoadMethod(ImportAttribute attr, MethodInfo method, Pointer<byte> addr)
		{
			Guard.AssertNotNull(attr as ImportCallAttribute);
			var callAttr = (ImportCallAttribute) attr;
			var options  = callAttr.CallOptions;

			CheckOptions(options, out var bind, out var addToMap);

			if (bind) {
				Global.Value.WriteWarning("Binding {Name}", method.Name);
				FunctionFactory.Managed.SetEntryPoint(method, addr);
			}

			if (addToMap) {
				var enclosing = method.DeclaringType;

				Guard.AssertNotNull(enclosing);

				var name = method.Name;

				if (name.StartsWith(GET_PROPERTY_PREFIX)) {
					// The nameof operator does not return the name with the get prefix
					Format.Remove(ref name, GET_PROPERTY_PREFIX);
				}

				m_typeImportMaps[enclosing].Add(name, addr);
			}
		}


		private static T LoadComponents<T>(T value, Type type, IImportProvider ip, LoadFunction methodFn)
		{
			(MemberInfo[] members, ImportAttribute[] attributes) = type.GetAnnotated<ImportAttribute>();

			int lim = attributes.Length;

			if (lim == default) {
				return value;
			}

			for (var i = 0; i < lim; i++) {
				var attr = attributes[i];
				var mem  = members[i];

				// Resolve the symbol

				string        id   = ResolveIdentifier(attr, mem, out _);
				Pointer<byte> addr = ip.GetAddress(id);

				switch (mem.MemberType) {
					case MemberTypes.Property:
						var propInfo = (PropertyInfo) mem;
						var get      = propInfo.GetMethod;
						methodFn(attr, get, addr);
						break;
					case MemberTypes.Method:

						FindOptimization(attr, mem);

						// The import is a function or (ctor)
						methodFn(attr, (MethodInfo) mem, addr);
						break;
				}

				//Global.Value.WriteVerbose(null, "Loaded member {Id} @ {Addr}", id, addr);
			}

			return value;
		}

		private T LoadComponents<T>(T value, Type type, IImportProvider ip)
		{
			return LoadComponents(value, type, ip, LoadMethod);
		}

		#endregion
	}
}
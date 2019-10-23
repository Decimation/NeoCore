using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using JetBrains.Annotations;
using NeoCore.Assets;
using NeoCore.Import.Attributes;
using NeoCore.Interop;
using NeoCore.Memory;
using NeoCore.Model;
using NeoCore.Utilities;
using NeoCore.Utilities.Diagnostics;

// ReSharper disable ParameterTypeCanBeEnumerable.Global

namespace NeoCore.Import
{
	// todo: cleanup
	
	public sealed class ImportManager : Releasable
	{
		#region Constants

		private const string GET_PROPERTY_PREFIX      = "get_";
		private const string GET_PROPERTY_REPLACEMENT = "Get";

		protected override string Id => nameof(ImportManager);

		private static readonly string MapError =
			$"Map must static, readonly, and of type {typeof(ImportMap)}";

		private static readonly string NamespaceError = 
			$"Type must be decorated with \"{nameof(ImportNamespaceAttribute)}\"";
		
		private delegate void LoadMethodFunction(ImportAttribute attr, MethodInfo memberInfo, Pointer<byte> ptr);
		

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
			if (m_boundTypes.Count != 0 || m_typeImportMaps.Count != 0) {
				Guard.Fail();
			}

			// Delete instance
			Value = null;

			base.Close();
		}

		#endregion

		#region Fields

		private readonly ISet<Type> m_boundTypes = new HashSet<Type>();

		private readonly Dictionary<Type, ImportMap> m_typeImportMaps = new Dictionary<Type, ImportMap>();

		#endregion

		#region Helper

		internal static string Combine(params string[] args)
		{
			const string SCOPE_RESOLUTION_OPERATOR = "::";

			var sb = new StringBuilder();

			for (int i = 0; i < args.Length; i++) {
				sb.Append(args[i]);

				if (i + 1 != args.Length) {
					sb.Append(SCOPE_RESOLUTION_OPERATOR);
				}
			}

			return sb.ToString();
		}

		private bool IsBound(Type t) => m_boundTypes.Contains(t);

		private void VerifyImport(ImportAttribute attr, MemberInfo member)
		{
			switch (member.MemberType) {
				case MemberTypes.Constructor:
				case MemberTypes.Property:
				case MemberTypes.Method:

					if (!(attr is ImportCallAttribute)) {
						Guard.Fail();
					}

					break;
				case MemberTypes.Field:

					if (!(attr is ImportFieldAttribute)) {
						Guard.Fail();
					}

					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private static bool IsAnnotated(Type t, out ImportNamespaceAttribute attr)
		{
			attr = t.GetCustomAttribute<ImportNamespaceAttribute>();

			return attr != null;
		}

		

		private static string ResolveIdentifier(ImportAttribute attr, [NotNull] MemberInfo member)
		{
			return ResolveIdentifier(attr, member, out _);
		}

		private static string ResolveIdentifier(ImportAttribute attr, [NotNull] MemberInfo member,
		                                        out string      resolvedId)
		{
			Guard.AssertNotNull(member.DeclaringType, nameof(member.DeclaringType));
			
			if (!IsAnnotated(member.DeclaringType, out var nameSpaceAttr)) {
				Guard.Fail(NamespaceError);
			}

			// Resolve the symbol

			resolvedId = attr.Identifier ?? member.Name;

			string nameSpace          = nameSpaceAttr.Namespace;
			string enclosingNamespace = member.DeclaringType.Name;

			var options = attr.Options;

			if (member.MemberType == MemberTypes.Method
			    && attr is ImportCallAttribute callAttr
			    && callAttr.CallOptions.HasFlagFast(ImportCallOptions.Constructor)) {
				if (!options.HasFlagFast(IdentifierOptions.FullyQualified)) {
					Guard.Fail(
						$"\"{nameof(IdentifierOptions)}\" must be \"{nameof(IdentifierOptions.FullyQualified)}\"");
				}

				// return enclosingNamespace + SCOPE_RESOLUTION_OPERATOR + enclosingNamespace;
				return Combine(enclosingNamespace, enclosingNamespace);
			}


			if (!options.HasFlagFast(IdentifierOptions.IgnoreEnclosingNamespace)) {
				// resolvedId = enclosingNamespace + SCOPE_RESOLUTION_OPERATOR + resolvedId;
				resolvedId = Combine(enclosingNamespace, resolvedId);
			}

			if (!options.HasFlagFast(IdentifierOptions.IgnoreNamespace)) {
				if (nameSpace != null) {
					// resolvedId = nameSpace + SCOPE_RESOLUTION_OPERATOR + resolvedId;
					resolvedId = Combine(nameSpace, resolvedId);
				}
			}

			if (options.HasFlagFast(IdentifierOptions.UseAccessorName)) {
				Guard.Assert(member.MemberType == MemberTypes.Method);
				resolvedId = resolvedId.Replace(GET_PROPERTY_PREFIX, GET_PROPERTY_REPLACEMENT);
			}

			Guard.AssertNotNull(resolvedId, nameof(resolvedId));

			return resolvedId;
		}

		#endregion

		#region Unload

		private void UnloadMap(Type type, FieldInfo mapField)
		{
			var map = m_typeImportMaps[type];

			map.Clear();

			m_typeImportMaps.Remove(type);

			mapField.SetValue(null, null);

			// Sanity check
			Guard.AssertDebug(!m_typeImportMaps.ContainsKey(type));
			Guard.AssertDebug(mapField.GetValue(null) == null);
		}

		/// <summary>
		/// Root unload function. Unloads and restores the type <paramref name="type"/>.
		/// </summary>
		public void Unload(Type type)
		{
			if (!IsBound(type)) {
				return;
			}

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

		private void LoadMap(Type t, FieldInfo field)
		{
			if (!field.IsStatic || field.FieldType != typeof(ImportMap)) {
				Guard.Fail(MapError);
			}

			var map = (ImportMap) field.GetValue(null);
			m_typeImportMaps.Add(t, map);
		}


		private FieldInfo FindMapField(Type type)
		{
			var mapField = type.GetAnyField(ImportMap.FIELD_NAME);

			if (mapField != null && mapField.GetCustomAttribute<ImportMapFieldAttribute>() == null) {
				Guard.Fail(
					$"Map field should be annotated with {nameof(ImportMapFieldAttribute)}");
			}

			if (mapField == null) {
				var (member, _) = type.GetFirstAnnotated<ImportMapFieldAttribute>();

				if (member != null) {
					mapField = (FieldInfo) member;
				}
			}

			return mapField;
		}

		private static bool CheckImportMap(FieldInfo mapField)
		{
			return mapField.IsStatic							// Must be static
			       && mapField.IsInitOnly 						// Must be readonly
			       && mapField.FieldType == typeof(ImportMap); 	// Must be of type ImportMap
		}

		private bool UsingMap(Type type, out FieldInfo mapField)
		{
			mapField = FindMapField(type);

			bool exists = mapField != null;

			if (exists) {
				if (!CheckImportMap(mapField)) {
					Guard.Fail(MapError);
				}

				if (mapField.GetValue(null) == null) {
					Guard.Fail($"{typeof(ImportMap)} is null");
				}
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

			if (!IsAnnotated(type, out _)) {
				Guard.Fail(NamespaceError);
			}

			if (UsingMap(type, out var mapField)) {
				LoadMap(type, mapField);
				value = LoadComponents(value, type, ip, LoadMethod);
			}
			else {
				value = LoadComponents(value, type, ip);
			}

			m_boundTypes.Add(type);

			Global.Value.WriteVerbose(Id, "Completed loading {Name}", type.Name);

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

		// Shortcut
//		internal void LoadClr(Type t) => Load(t, Clr.Value.ClrSymbols);

		// Shortcut
//		internal void LoadAllClr(Type[] t) => LoadAll(t, Clr.Value.ClrSymbols);

		// Field loading is not yet rewritten


		private void LoadMethod(ImportAttribute attr, MethodInfo method, Pointer<byte> addr)
		{
			var callAttr = attr as ImportCallAttribute;
			Guard.AssertNotNull(callAttr, nameof(callAttr));
			var options = callAttr.CallOptions;

			if (options == ImportCallOptions.None) {
				Guard.Fail("You must specify an option");
			}
			
			bool bind     = options.HasFlagFast(ImportCallOptions.Bind);
			bool addToMap = options.HasFlagFast(ImportCallOptions.Map);

			if (bind && addToMap) {
				Guard.Fail($"The option {ImportCallOptions.Bind} cannot be used with {ImportCallOptions.Map}");
			}

			if (bind) {
				Global.Value.WriteWarning("Binding {Name}", method.Name);
				FunctionFactory.Managed.SetEntryPoint(method, addr);
			}

			if (addToMap) {
				var enclosing = method.DeclaringType;

				if (enclosing == null) {
					Guard.Fail();
				}

				var name = method.Name;

				if (name.StartsWith(GET_PROPERTY_PREFIX)) {
					// The nameof operator does not return the name with the get prefix
					Format.Remove(ref name,GET_PROPERTY_PREFIX);
				}


				m_typeImportMaps[enclosing].Add(name, addr);
			}
		}


		private static T LoadComponents<T>(T                    value,
		                                   Type                 type,
		                                   IImportProvider      ip,
		                                   LoadMethodFunction   methodFn)
		{
			(MemberInfo[] members, ImportAttribute[] attributes) = type.GetAnnotated<ImportAttribute>();

			int lim = attributes.Length;

			if (lim == default) {
				return value;
			}

			for (int i = 0; i < lim; i++) {
				var attr = attributes[i];
				var mem  = members[i];

				// Resolve the symbol

				string        id   = ResolveIdentifier(attr, mem);
				Pointer<byte> addr = ip.GetAddress(id);

				switch (mem.MemberType) {
					case MemberTypes.Property:
						var propInfo = (PropertyInfo) mem;
						var get      = propInfo.GetMethod;
						methodFn(attr, get, addr);
						break;
					case MemberTypes.Method:
						// The import is a function or (ctor)
						methodFn(attr, (MethodInfo) mem, addr);
						break;
				}

				Global.Value.WriteVerbose(null, "Loaded member {Id} @ {Addr}", id, addr);
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
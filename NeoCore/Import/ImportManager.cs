using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using JetBrains.Annotations;
using Memkit;
using Memkit.Pointers;
using Memkit.Utilities;
using NeoCore.CoreClr;
using NeoCore.CoreClr.Meta;
using NeoCore.Import.Attributes;
using NeoCore.Model;
using NeoCore.Utilities;
using NeoCore.Utilities.Diagnostics;
using static NeoCore.Utilities.Format;
using EasyReflection = NeoCore.Utilities.EasyReflection;

// ReSharper disable ParameterTypeCanBeEnumerable.Global

namespace NeoCore.Import
{
	/// <summary>
	/// Loads, links, and dynamically binds imports to the decorated members in a type.
	/// </summary>
	public sealed partial class ImportManager : Releasable
	{
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

		#region Constants

		protected override string Id => nameof(ImportManager);

		private const string GET_PROPERTY_REPLACEMENT = "Get";

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

		#region Fields

		/// <summary>
		/// Types bound or ignored
		/// </summary>
		private readonly ISet<Type> m_completeTypes = new HashSet<Type>();

		private readonly Dictionary<Type, ImportMap> m_typeImportMaps = new Dictionary<Type, ImportMap>();

		#endregion


		#region Unload

		private void UnloadMap(Type type, FieldInfo mapField)
		{
			var map = m_typeImportMaps[type];

			map.Clear();

			m_typeImportMaps.Remove(type);

			var value = (ImportMap) mapField.GetValue(null);
			//mapField.SetValue(null, null);
			value?.Clear();


			// Sanity check
			CheckMapFieldUnload(type, mapField);
		}

		/// <summary>
		/// Root unload function. Unloads and restores the type <paramref name="type"/>.
		/// </summary>
		public void Unload(Type type)
		{
			if (!IsBoundOrIgnored(type)) {
				return;
			}

			if (UsingMap(type, out var mapField)) {
				UnloadMap(type, mapField);
			}


			var components = type.GetAnnotated<ImportAttribute>();

			int lim = components.Length;

			for (int i = 0; i < lim; i++) {
				var mem  = components[i].Member;
				var attr = components[i].Attribute;


				switch (mem.MemberType) {
					case MemberTypes.Property:
						var propInfo = (PropertyInfo) mem;
						var get      = propInfo.GetMethod;


						break;
					case MemberTypes.Field:
						// The field will be deleted later
						var fi = (FieldInfo) mem;
						if (fi.IsStatic && !fi.IsInitOnly) {
							fi.SetValue(null, default);
						}

						break;
					case MemberTypes.Method:


						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}

			m_completeTypes.Remove(type);

//			CoreLogger.Value.WriteInfo(null, "Unloaded {Name}", type.Name);
		}

		public void Unload<T>(ref T value)
		{
			var type = value.GetType();

			Unload(type);

			Runtime.Destroy(ref value);
		}

		public void UnloadAll(Type[] t)
		{
			foreach (var type in t) {
				Unload(type);
			}
		}

		public void UnloadAll() => UnloadAll(m_completeTypes.ToArray());

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
				var member = type.GetFirstAnnotated<ImportMapFieldAttribute>().Member;

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

		#region Load field

		private static object ProxyLoadField(ImportFieldAttribute ifld, MetaField field, Pointer<byte> ptr)
		{
			var fieldLoadType = (MetaType) (ifld.LoadAs ?? field.FieldType.RuntimeType);

			return fieldLoadType.IsAnyPointer ? ptr : ptr.ReadAny(fieldLoadType.RuntimeType);
		}

		private static void FastLoadField(MetaField fieldInfo, Pointer<byte> addr, Pointer<byte> fieldAddr)
		{
			int    fieldSize = fieldInfo.Size;
			byte[] memCpy    = addr.Cast().Copy(fieldSize);
			fieldAddr.WriteAll(memCpy);
		}

		#endregion

		/// <summary>
		///     Root load function. Loads <paramref name="value" /> of type <paramref name="type" /> using the
		///     specified <see cref="ImportProvider" /> <paramref name="ip" />.
		/// </summary>
		/// <param name="value">Value of type <paramref name="type" /> to load</param>
		/// <param name="type"><see cref="MetaType" /> of <paramref name="value" /></param>
		/// <param name="ip"><see cref="ImportProvider" /> to use to load components</param>
		/// <typeparam name="T">Type of <paramref name="value" /></typeparam>
		/// <returns><paramref name="value" />, fully loaded</returns>
		private T Load<T>(T value, Type type, ImportProvider ip)
		{
			if (IsBoundOrIgnored(type)) {
				return value;
			}

			CheckAnnotations(type, false, out _);

			if (!ContainsAnnotatedMembers(type, out AnnotatedMember<ImportAttribute>[] components)) {
//				Global.Value.WriteWarning(null, "Load: {Name} has no members to import; ignoring", type.Name);
				m_completeTypes.Add(type);
				return value;
			}

			if (UsingMap(type, out var mapField)) {
				if (m_typeImportMaps.ContainsKey(type)) {
					return value;
				}

				AddMapToDictionary(type, mapField);
			}

			value = LoadComponents(value, ip, components, m_typeImportMaps);

			m_completeTypes.Add(type);

//			Global.Value.WriteInfo(null, "Loaded {Name}", type.Name);
			return value;
		}

		/// <summary>
		///     Loads <paramref name="value" /> using <paramref name="ip" />.
		/// </summary>
		/// <param name="value">Value to load</param>
		/// <param name="ip"><see cref="ImportProvider" /> to use</param>
		/// <typeparam name="T">Type of <paramref name="value" /></typeparam>
		/// <returns><paramref name="value" />, fully loaded</returns>
		public T Load<T>(T value, ImportProvider ip) => Load(value, value.GetType(), ip);

		/// <summary>
		///     Loads any non-instance components of type <paramref name="t" />.
		/// </summary>
		/// <param name="t"><see cref="Type" /> to load</param>
		/// <param name="ip"><see cref="ImportProvider" /> to use</param>
		/// <returns>A <c>default</c> object of type <paramref name="t" /></returns>
		public object Load(Type t, ImportProvider ip) => Load(default(object), t, ip);

		public void LoadAll(Type[] t, ImportProvider ip)
		{
			foreach (var type in t) {
				Load(type, ip);
			}
		}

		private static void LoadFieldComponent<T>(ImportAttribute attr, ref T value,
		                                          ImportProvider ip,
		                                          string          identifier,
		                                          MetaField       field)
		{
			var           ifld      = (ImportFieldAttribute) attr;
			Pointer<byte> ptr       = ip.GetAddress(identifier);
			var           options   = ifld.FieldOptions;
			Pointer<byte> fieldAddr = field.GetValueAddress(ref value);


			if (ifld is ImportGlobalFieldAttribute) {
				CheckGlobalField(field);
			}


			object fieldValue;

			switch (options) {
				case ImportFieldOptions.Proxy:
					fieldValue = ProxyLoadField(ifld, field, ptr);
					break;
				case ImportFieldOptions.Fast:
					FastLoadField(field, ptr, fieldAddr);
					return;
				default:
					throw new ArgumentOutOfRangeException();
			}

			if (field.FieldType.IsAnyPointer) {
				ptr.WritePointer((Pointer<byte>) fieldValue);
			}
			else {
				ptr.WriteAny(field.FieldType.RuntimeType, fieldValue);
			}
		}

		private static void LoadMethodComponent(ImportAttribute             attr,
		                                        MethodInfo                  method,
		                                        Pointer<byte>               addr,
		                                        Dictionary<Type, ImportMap> importMaps)
		{
			Guard.AssertNotNull(attr as ImportCallAttribute);
			var callAttr = (ImportCallAttribute) attr;
			var options  = callAttr.CallOptions;

			CheckOptions(options, out bool addToMap);

			if (addToMap) {
				var enclosing = method.DeclaringType;

				Guard.AssertNotNull(enclosing);

				var name = method.Name;

				if (name.StartsWith(EasyReflection.GET_PROPERTY_PREFIX)) {
					// The nameof operator does not return the name with the get prefix
					name = name.Replace(EasyReflection.GET_PROPERTY_PREFIX, String.Empty);
				}

				importMaps[enclosing].Add(name, addr);
			}
		}


		private static T LoadComponents<T>(T                                  value,
		                                   ImportProvider                    ip,
		                                   AnnotatedMember<ImportAttribute>[] components,
		                                   Dictionary<Type, ImportMap>        importMaps)
		{
			int lim = components.Length;

			if (lim == default) {
				return value;
			}

			for (int i = 0; i < lim; i++) {
				var attr = components[i].Attribute;
				var mem  = components[i].Member;

				// Resolve the symbol

				string        id   = ResolveIdentifier(attr, mem, out _);
				Pointer<byte> addr = ip.GetAddress(id);

				switch (mem.MemberType) {
					case MemberTypes.Property:
						var propInfo = (PropertyInfo) mem;
						var get      = propInfo.GetMethod;
						LoadMethodComponent(attr, get, addr, importMaps);
						break;
					case MemberTypes.Method:

						FindOptimization(attr, mem);

						LoadMethodComponent(attr, (MethodInfo) mem, addr, importMaps);
						break;
					case MemberTypes.Field:
						LoadFieldComponent(attr, ref value, ip, id, (MetaField) mem);
						break;
				}

//				Global.Value.WriteVerbose(null, "{Check} Loaded member {Id} @ {Addr}", '\u2714',id, addr);
			}

			return value;
		}

		#endregion
	}
}
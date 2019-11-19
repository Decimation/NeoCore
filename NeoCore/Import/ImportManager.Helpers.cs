using System;
using System.Reflection;
using JetBrains.Annotations;
using NeoCore.Assets;
using NeoCore.CoreClr.Meta;
using NeoCore.Import.Attributes;
using NeoCore.Model;
using NeoCore.Utilities;
using NeoCore.Utilities.Diagnostics;
using NeoCore.Utilities.Extensions;

namespace NeoCore.Import
{
	public sealed partial class ImportManager
	{
		private bool IsBound(Type t) => m_boundTypes.Contains(t);

		
		internal static string ScopeJoin(string[] scopes) =>
			String.Join(Format.Constants.SCOPE_RESOLUTION_OPERATOR, scopes);

		private static bool IsAnnotated(MemberInfo t, out ImportNamespaceAttribute attr)
		{
			attr = t.GetCustomAttribute<ImportNamespaceAttribute>();

			return attr != null;
		}

		private static bool ContainsAnnotatedMembers(Type type, out AnnotatedMember<ImportAttribute>[] components)
		{
			components = type.GetAnnotated<ImportAttribute>();

			return components != null && components.Length > 0;
		}

		private static bool IsImportMapQualified(FieldInfo mapField)
		{
			// Field must be static, readonly, and of type ImportMap

			return mapField.IsStatic && mapField.IsInitOnly && mapField.FieldType == typeof(ImportMap);
		}

		[AssertionMethod]
		private void CloseCheck()
		{
			if (m_boundTypes.Count != 0 || m_typeImportMaps.Count != 0) {
				Guard.Fail();
			}
		}

		[AssertionMethod]
		private static void CheckGlobalField(MetaField field)
		{
			Guard.Assert(field.IsStatic && field.FieldInfo.IsInitOnly);
		}

		[AssertionMethod]
		private void CheckMapFieldUnload(Type t, FieldInfo mapField)
		{
			Guard.AssertDebug(!m_typeImportMaps.ContainsKey(t));
			//Guard.AssertDebug(mapField.GetValue(null) == null);
		}

		[AssertionMethod]
		private static void CheckAnnotations(MemberInfo                   member,
		                                     bool                         nameSpace,
		                                     out ImportNamespaceAttribute nameSpaceAttr)
		{
			var t = nameSpace ? member.DeclaringType : (Type) member;

			if (!IsAnnotated(t, out nameSpaceAttr)) {
				string namespaceError =
					$"Type \"{t?.Name}\" must be decorated with \"{nameof(ImportNamespaceAttribute)}\"";
				Guard.Fail(namespaceError);
			}
		}

		[AssertionMethod]
		private static void CheckImportMap(FieldInfo mapField)
		{
			if (!IsImportMapQualified(mapField)) {
				string mapError = $"Map must static, readonly, and of type {typeof(ImportMap)}";
				Guard.Fail(mapError);
			}

			if (mapField.GetValue(null) == null) {
				Guard.Fail($"{typeof(ImportMap)} is null");
			}
		}


		[AssertionMethod]
		private static void CheckImportMapAnnotation(FieldInfo mapField)
		{
			if (mapField != null && mapField.GetCustomAttribute<ImportMapFieldAttribute>() == null) {
				Guard.Fail($"Map field should be annotated with {nameof(ImportMapFieldAttribute)}");
			}
		}

		[AssertionMethod]
		private static void CheckConstructorOptions(IdentifierOptions options)
		{
			if (!options.HasFlagFast(IdentifierOptions.FullyQualified)) {
				Guard.Fail(
					$"\"{nameof(IdentifierOptions)}\" must be \"{nameof(IdentifierOptions.FullyQualified)}\"");
			}
		}

		[AssertionMethod]
		private static void CheckOptions(ImportCallOptions options, out bool addToMap)
		{
			if (options == ImportCallOptions.None) {
				Guard.Fail("You must specify an option");
			}
			
			addToMap = options.HasFlagFast(ImportCallOptions.Map);
		}


		private static void FindOptimization(ImportAttribute import, MemberInfo mem)
		{
			if (import is ImportCallAttribute callAttr && !(callAttr is ImportAccessorAttribute)) {
				bool warn = callAttr.CallOptions == ImportCallOptions.Map
				            && callAttr.Options == IdentifierOptions.UseAccessorName;

				if (warn) {
					CoreLogger.Value.WriteWarning(null, "Use {Name} on member {Member} in {Type}",
					                              nameof(ImportAccessorAttribute),
					                              mem.Name, mem.DeclaringType?.Name);
				}
			}
		}
	}
}
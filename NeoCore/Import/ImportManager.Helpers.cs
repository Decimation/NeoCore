using System;
using System.Reflection;
using JetBrains.Annotations;
using NeoCore.Assets;
using NeoCore.Import.Attributes;
using NeoCore.Model;
using NeoCore.Utilities;
using NeoCore.Utilities.Diagnostics;

namespace NeoCore.Import
{
	public sealed partial class ImportManager
	{
		private static readonly string NamespaceError =
			$"Type must be decorated with \"{nameof(ImportNamespaceAttribute)}\"";

		private bool IsBound(Type t) => m_boundTypes.Contains(t);

		private static bool IsAnnotated(Type t, out ImportNamespaceAttribute attr)
		{
			attr = t.GetCustomAttribute<ImportNamespaceAttribute>();

			return attr != null;
		}

		private static bool IsImportMapQualified(FieldInfo mapField)
		{
			// Field must be static, readonly, and of type ImportMap

			return mapField.IsStatic && mapField.IsInitOnly && mapField.FieldType == typeof(ImportMap);
		}

		[AssertionMethod]
		private static void CheckNamespaceAnnotation(MemberInfo member, out ImportNamespaceAttribute nameSpaceAttr)
		{
			if (!IsAnnotated(member.DeclaringType, out nameSpaceAttr)) {
				Guard.Fail(NamespaceError);
			}
		}

		[AssertionMethod]
		private static void CheckAnnotation(Type t)
		{
			if (!IsAnnotated(t, out _)) {
				Guard.Fail(NamespaceError);
			}
		}

		[AssertionMethod]
		private static void CheckAnnotations(MemberInfo                   member,
		                                     bool                         nameSpace,
		                                     out ImportNamespaceAttribute nameSpaceAttr)
		{
			Type t = nameSpace ? member.DeclaringType : (Type) member;
			
			if (!IsAnnotated(t, out nameSpaceAttr)) {
				Guard.Fail(NamespaceError);
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
				Guard.Fail(
					$"Map field should be annotated with {nameof(ImportMapFieldAttribute)}");
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
		private static void CheckOptions(ImportCallOptions options, out bool bind, out bool addToMap)
		{
			if (options == ImportCallOptions.None) {
				Guard.Fail("You must specify an option");
			}

			bind     = options.HasFlagFast(ImportCallOptions.Bind);
			addToMap = options.HasFlagFast(ImportCallOptions.Map);

			if (bind && addToMap) {
				Guard.Fail($"The option {ImportCallOptions.Bind} cannot be used with {ImportCallOptions.Map}");
			}
		}


		private static void FindOptimization(ImportAttribute import, MemberInfo mem)
		{
			if (import is ImportCallAttribute callAttr && !(callAttr is ImportPropertyAttribute)) {
				bool warn = callAttr.CallOptions == ImportCallOptions.Map
				            && callAttr.Options == IdentifierOptions.UseAccessorName;

				if (warn) {
					Global.Value.WriteWarning(null, "Use {Name} on member {Member} in {Type}",
					                          nameof(ImportPropertyAttribute),
					                          mem.Name, mem.DeclaringType?.Name);
				}
			}
		}
	}
}
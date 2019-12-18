using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using NeoCore.Utilities.Diagnostics;
using NeoCore.Utilities.Extensions;

namespace NeoCore.Utilities
{
	/// <summary>
	/// <seealso cref="ReflectionExtensions"/>
	/// </summary>
	public static class EasyReflection
	{
		#region Flags

		/// <summary>
		///     <see cref="ALL_INSTANCE_FLAGS" /> and <see cref="BindingFlags.Static" />
		/// </summary>
		public const BindingFlags ALL_FLAGS = ALL_INSTANCE_FLAGS | BindingFlags.Static;

		/// <summary>
		///     <see cref="BindingFlags.Public" />, <see cref="BindingFlags.Instance" />,
		///     and <see cref="BindingFlags.NonPublic" />
		/// </summary>
		private const BindingFlags ALL_INSTANCE_FLAGS =
			BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic;

		#endregion

		#region Identifiers

		// Microsoft.VisualBasic.Core/src/Microsoft/VisualBasic/CompilerServices/Symbols.vb

		public static string GetGenericTypeName(Type t)
		{
			const char GRAVE_ACCENT  = '`';
			const char LEFT_CHEVRON  = '<';
			const char RIGHT_CHEVRON = '>';

			var sb = new StringBuilder();

			string[] genericTypeNames = t.GenericTypeArguments.Select(g => g.Name).ToArray();

			return sb.Append(t.Name.Split(GRAVE_ACCENT).First())
			         .Append(LEFT_CHEVRON)
			         .Append(String.Join(Format.JOIN_COMMA, genericTypeNames))
			         .Append(RIGHT_CHEVRON)
			         .ToString();
		}

		/// <summary>
		/// Returns the internal metadata name of a property's backing field.
		/// </summary>
		/// <param name="name">Regular field name (name of the property)</param>
		/// <returns>Actual name of the backing field.</returns>
		public static string GetBackingFieldName(string name) =>
			CreateNameBracket(name) + BACKING_FIELD_NAME_STUB;


		/// <summary>
		/// Returns the internal metadata type name of a fixed buffer field.
		/// </summary>
		/// <param name="name">Regular field name (name of the fixed buffer)</param>
		/// <returns>Actual type name of the buffer field.</returns>
		public static string GetBufferTypeName(string name) =>
			CreateNameBracket(name) + FIXED_BUFFER_TYPE_NAME_STUB;


		private static string CreateNameBracket(string name)
		{
			const string NAME_FORMAT_BRACKET = "<{0}>";
			return String.Format(NAME_FORMAT_BRACKET, name);
		}

		/// <summary>
		/// Unknown
		/// </summary>
		public const string CLOSURE_CLASS_NAME_STUB = "<>c__DisplayClass";

		/// <summary>
		/// Unknown
		/// </summary>
		public const string ARRAY_SIZE_CLASS_NAME_STUB = "__StaticArrayInitTypeSize=";

		public const string FIXED_BUFFER_TYPE_NAME_STUB = "e__FixedBuffer";

		public const string BACKING_FIELD_NAME_STUB = "k__BackingField";

		public const string GET_PROPERTY_PREFIX = "get_";

		public const string SET_PROPERTY_PREFIX = "set_";

		public const string OPERATOR_METHOD_PREFIX = "op_";

		#endregion
	}
}
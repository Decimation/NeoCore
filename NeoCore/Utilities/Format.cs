using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NeoCore.Utilities.Extensions;

namespace NeoCore.Utilities
{
	/// <summary>
	/// Contains utilities for formatting.
	/// </summary>
	public static partial class Format
	{
		/// <summary>
		/// Returns the internal metadata name of a property's backing field.
		/// </summary>
		/// <param name="name">Regular field name (name of the property)</param>
		/// <returns>Actual name of the backing field.</returns>
		public static string GetBackingFieldName(string name)
		{
			return String.Format("<{0}>k__BackingField", name);
		}
		
		public const string PDB_EXT = ".pdb";
		public const string DLL_EXT = ".dll";

		#region Join

		public const string JOIN_COMMA = ", ";

		public const string JOIN_SPACE = " ";

		/// <summary>
		/// Scope resolution operator
		/// </summary>
		public const string JOIN_SCOPE = "::";

		/// <summary>
		/// Concatenates the strings returned by <paramref name="toString"/>
		/// using the specified separator between each element or member.
		/// </summary>
		/// <param name="values">Collection of values</param>
		/// <param name="toString">Function which returns a <see cref="string"/> given a member of <paramref name="values"/></param>
		/// <param name="delim">Delimiter</param>
		/// <typeparam name="T">Element type</typeparam>
		public static string FuncJoin<T>(IEnumerable<T>  values,
		                                 Func<T, string> toString,
		                                 string          delim = JOIN_COMMA)
		{
			return String.Join(delim, values.Select(toString));
		}

		public static string FormatJoin<T>(IEnumerable<T>  values,          string format,
		                                   IFormatProvider provider = null, string delim = JOIN_COMMA)
			where T : IFormattable
		{
			return String.Join(delim, values.Select(v => v.ToString(format, provider)));
		}

		public static string SimpleJoin<T>(IEnumerable<T> values, string delim = JOIN_COMMA) =>
			String.Join(delim, values);

		public static string ToString<T>(T[] rg)
		{
			if (typeof(T) == typeof(byte)) {
				var byteArray = rg as byte[];
				return FormatJoin(byteArray, HEX_FORMAT_SPECIFIER);
			}

			return SimpleJoin(rg);
		}

		#endregion

		#region Hex

		private const string HEX_FORMAT_SPECIFIER = "X";

		private const string HEX_PREFIX = "0x";

		public static string ToHexString<T>(T value, HexOptions options = HexOptions.Default) where T : IFormattable
		{
			return ToHexString(value, null, options);
		}

		public static string ToHexString<T>(T          value, IFormatProvider provider = null,
		                                    HexOptions options = HexOptions.Default) where T : IFormattable
		{
			var sb = new StringBuilder();

			if (options.HasFlagFast(HexOptions.Prefix)) {
				sb.Append(HEX_PREFIX);
			}

			string hexStr = value.ToString(HEX_FORMAT_SPECIFIER, provider);

			if (options.HasFlagFast(HexOptions.Lowercase)) {
				hexStr = hexStr.ToLower();
			}

			sb.Append(hexStr);

			return sb.ToString();
		}

		public static unsafe string ToHexString(void* value, HexOptions options = HexOptions.Default) =>
			ToHexString((long) value, options);

		public static unsafe string ToHexString(IntPtr value, HexOptions options = HexOptions.Default) =>
			ToHexString((long) value, options);

		#endregion
	}


	[Flags]
	public enum HexOptions
	{
		None = 0,

		Prefix = 1,

		Lowercase = 1 << 1,

		Default = Prefix
	}
}
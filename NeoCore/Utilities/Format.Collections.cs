using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using static NeoCore.Utilities.Format.Constants;

namespace NeoCore.Utilities
{
	public static partial class Format
	{
		/// <summary>
		/// Contains utilities for formatting <see cref="IEnumerable{T}"/> types.
		/// </summary>
		public static class Collections
		{
			

			/// <summary>
			/// Concatenates the strings returned by <paramref name="toString"/>
			/// using the specified separator between each element or member.
			/// </summary>
			/// <param name="values">Collection of values</param>
			/// <param name="toString">Function which returns a <see cref="string"/> given a member of <paramref name="values"/></param>
			/// <param name="delim">Delimiter</param>
			/// <typeparam name="T">Element type</typeparam>
			public static string FuncJoin<T>(IEnumerable<T> values,
			                                 Func<T, string> toString,
			                                 string          delim = JOIN_COMMA)
			{
				return String.Join(delim, values.Select(toString));
			}

			public static string FormatJoin<T>(IEnumerable<T> values,
			                                   string          format,
			                                   IFormatProvider provider = null,
			                                   string          delim    = JOIN_COMMA) where T : IFormattable
			{
				return String.Join(delim, values.Select(v => v.ToString(format, provider)));
			}

			public static string SimpleJoin<T>(IEnumerable<T> values, string delim = JOIN_COMMA)
			{
				return String.Join(delim, values);
			}

			public static string ToString<T>(T[] rg)
			{
				if (typeof(T) == typeof(byte)) {
					var byteArray = rg as byte[];
					return Format.Collections.FormatJoin(byteArray, Hex.HEX_FORMAT_SPECIFIER);
				}

				return Format.Collections.SimpleJoin(rg);
			}
		}
	}
}
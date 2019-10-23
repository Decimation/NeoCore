using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NeoCore.Utilities
{
	public static partial class Format
	{
		/// <summary>
		/// Contains utilities for formatting <see cref="IEnumerable{T}"/> types.
		/// </summary>
		public static class Collections
		{
			private const string JOIN_COMMA = ", ";

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
		}
	}
}
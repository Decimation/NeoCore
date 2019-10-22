using System;
using System.Linq;

namespace NeoCore.Utilities
{
	public static class Format
	{
		private const string HEX_FORMAT_SPECIFIER = "X";
		private const string JOIN_COMMA           = ", ";

		public static unsafe string AsHex(void* value)
		{
			return ((long) value).ToString(HEX_FORMAT_SPECIFIER);
		}

		public static string FormatJoin<T>(T[] value, string format) where T : IFormattable
		{
			var formatStrings = value.Select(v => v.ToString(format, null)).ToArray();
			return String.Join(JOIN_COMMA, formatStrings);
		}

		public static string SimpleJoin<T>(T[] value)
		{
			return String.Join(JOIN_COMMA, value);
		}
	}
}
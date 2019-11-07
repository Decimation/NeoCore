using System;
using System.Text;
using NeoCore.Utilities.Extensions;

namespace NeoCore.Utilities
{
	public static partial class Format
	{
		public static class Hex
		{
			internal const string HEX_FORMAT_SPECIFIER = "X";

			internal const string HEX_PREFIX = "0x";

			public static string ToHexString<T>(T value, HexOptions options = HexOptions.Default) where T : IFormattable
			{
				return ToHexString(value, null, options);
			}

			public static string ToHexString<T>(T          value, IFormatProvider provider = null,
			                                    HexOptions options = HexOptions.Default) where T : IFormattable
			{
				var sb = new StringBuilder();

				if (options.HasFlagFast(HexOptions.Prefix)) {
					sb.Append(Hex.HEX_PREFIX);
				}

				string hexStr = value.ToString(Hex.HEX_FORMAT_SPECIFIER, provider);

				if (options.HasFlagFast(HexOptions.Lowercase)) {
					hexStr = hexStr.ToLower();
				}

				sb.Append(hexStr);

				return sb.ToString();
			}

			public static unsafe string ToHexString(void* value, HexOptions options = HexOptions.Default) =>
				Hex.ToHexString((long) value, options);

			public static unsafe string ToHexString(IntPtr value, HexOptions options = HexOptions.Default) =>
				Hex.ToHexString((long) value, options);
		}
	}
}
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
		private const string HEX_FORMAT_SPECIFIER = "X";

		private const string HEX_PREFIX = "0x";

		public static unsafe string ToHexString(void* value, HexOptions options = HexOptions.Default) =>
			ToHexString((IntPtr) value, options);

		public static string ToHexString(IntPtr value, HexOptions options = HexOptions.Default)
		{
			var sb = new StringBuilder();

			if (options.HasFlagFast(HexOptions.Prefix)) {
				sb.Append(HEX_PREFIX);
			}

			string hexStr = value.ToInt64().ToString(HEX_FORMAT_SPECIFIER);

			if (options.HasFlagFast(HexOptions.Lowercase)) {
				hexStr = hexStr.ToLower();
			}

			sb.Append(hexStr);

			return sb.ToString();
		}
		
		public static string GetBackingFieldName(string name)
		{
			return String.Format("<{0}>k__BackingField", name);
		}

		internal const string SCOPE_RESOLUTION_OPERATOR = "::";

		internal static string Combine(string[] args, string delim)
		{
			var sb = new StringBuilder();

			for (int i = 0; i < args.Length; i++) {
				sb.Append(args[i]);

				if (i + 1 != args.Length) {
					sb.Append(delim);
				}
			}

			return sb.ToString();
		}

		public static void Remove(ref string value, string substring)
		{
			value = value.Replace(substring, String.Empty);
		}
	}
}
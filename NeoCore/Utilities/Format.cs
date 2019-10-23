using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeoCore.Utilities
{
	/// <summary>
	/// Contains utilities for formatting.
	/// </summary>
	public static partial class Format
	{
		private const string HEX_FORMAT_SPECIFIER = "X";

		public static unsafe string AsHex(void* value) => AsHex((IntPtr) value);

		public static string AsHex(IntPtr value) => value.ToInt64().ToString(HEX_FORMAT_SPECIFIER);

		public static string GetBackingFieldName(string name)
		{
			return String.Format("<{0}>k__BackingField", name);
		}
		
		internal static string Combine(params string[] args)
		{
			const string SCOPE_RESOLUTION_OPERATOR = "::";

			var sb = new StringBuilder();

			for (int i = 0; i < args.Length; i++) {
				sb.Append(args[i]);

				if (i + 1 != args.Length) {
					sb.Append(SCOPE_RESOLUTION_OPERATOR);
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
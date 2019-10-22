using System;
using System.Collections.Generic;
using System.Linq;

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

		public static void Remove(ref string value, string substring)
		{
			value = value.Replace(substring, String.Empty);
		}
	}
}
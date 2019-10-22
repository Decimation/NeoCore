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

		public static string AsHex(IntPtr value)
		{
			return ((long) value).ToString(HEX_FORMAT_SPECIFIER);
		}
	}
}
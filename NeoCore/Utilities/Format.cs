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
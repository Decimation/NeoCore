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

		public static void Remove(ref string value, string substring)
		{
			value = value.Replace(substring, String.Empty);
		}
	}
}
using System.Collections.Generic;

// ReSharper disable ParameterTypeCanBeEnumerable.Global

namespace NeoCore.Utilities.Extensions
{
	public static class CollectionExtensions
	{
		public static void AddRange<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey[] keys, TValue value)
		{
			foreach (var key in keys) {
				dict.Add(key, value);
			}
		}
	}
}
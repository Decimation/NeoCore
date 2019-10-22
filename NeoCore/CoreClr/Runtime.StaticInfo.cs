using System;

namespace NeoCore.CoreClr
{
	public static partial class Runtime
	{
		/// <summary>
		/// Compile-time counterpart of <see cref="Runtime.Info"/>
		/// </summary>
		public static class StaticInfo
		{
			// todo: this is organized oddly
			
			internal static bool IsString<T>() => typeof(T) == typeof(string);
			
			internal static bool IsArray<T>()  => typeof(T).IsArray || typeof(T) == typeof(Array);
			
			internal static bool IsStruct<T>() => typeof(T).IsValueType;
		}
	}
}
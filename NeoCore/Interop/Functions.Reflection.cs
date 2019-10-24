using System;
using NeoCore.FastReflection;
using NeoCore.Utilities;

namespace NeoCore.Interop
{
	public static unsafe partial class Functions
	{
		public static class Reflection
		{
			public static TDelegate FindFunction<TDelegate, TSource>(string name) where TDelegate : Delegate
			{
				return FindFunction<TDelegate>(typeof(TSource), name);
			}
			
			public static TDelegate FindFunction<TDelegate>(Type type, string name) where TDelegate : Delegate
			{
				var method = type.GetAnyMethod(name);

				return (TDelegate) method.CreateDelegate(typeof(TDelegate));
			}
		}
	}
}
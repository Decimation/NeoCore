using System;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using NeoCore.CoreClr.Components.Support;
using NeoCore.Interop.Attributes;

namespace NeoCore.Interop
{
	public static unsafe partial class Functions
	{
		internal static class Clr
		{
			static Clr()
			{
				ReadTypeFromHandle = Reflection.FindFunction<GetTypeFromHandleFunc>();
				IsPinnable         = Reflection.FindFunction<IsPinnableFunc>();
			}

			[ReflectionFunction(typeof(Marshal), "IsPinnable")]
			internal delegate bool IsPinnableFunc([CanBeNull] object handle);

			[ReflectionFunction(typeof(Type), "GetTypeFromHandleUnsafe")]
			internal delegate Type GetTypeFromHandleFunc(IntPtr handle);

			internal static GetTypeFromHandleFunc ReadTypeFromHandle { get; }

			internal static IsPinnableFunc IsPinnable { get; }
		}
	}
}
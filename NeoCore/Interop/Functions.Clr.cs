using System;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using NeoCore.CoreClr.Components.Support;
using NeoCore.Interop.Attributes;

namespace NeoCore.Interop
{
	public static partial class Functions
	{
		/// <summary>
		/// Contains delegates for managed internal System functions which interact between the managed-unmanaged
		/// (System-CLR) boundary.
		/// </summary>
		internal static class Clr
		{
			static Clr()
			{
				GetTypeFromHandle = Reflection.FindFunction<GetTypeFromHandleDelegate>();
				IsPinnable        = Reflection.FindFunction<IsPinnableDelegate>();
			}

			[ReflectionFunction(typeof(Marshal), "IsPinnable")]
			internal delegate bool IsPinnableDelegate([CanBeNull] object handle);

			internal static IsPinnableDelegate IsPinnable { get; }

			[ReflectionFunction(typeof(Type), "GetTypeFromHandleUnsafe")]
			internal delegate Type GetTypeFromHandleDelegate(IntPtr handle);

			internal static GetTypeFromHandleDelegate GetTypeFromHandle { get; }
		}
	}
}
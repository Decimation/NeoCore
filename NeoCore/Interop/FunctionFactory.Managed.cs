using System;
using System.Reflection;
using NeoCore.CoreClr;
using NeoCore.CoreClr.Components.VM;
using NeoCore.Import;
using NeoCore.Import.Attributes;
using NeoCore.Memory;
using NeoCore.Memory.Pointers;
using NeoCore.Utilities.Diagnostics;

namespace NeoCore.Interop
{
	public static unsafe partial class FunctionFactory
	{
		/// <summary>
		/// Provides functions for resetting and setting the entry point for managed methods.
		/// </summary>
		internal static class Managed
		{
			/// <summary>
			/// Resets the method represented by <paramref name="mi"/> to its original, blank state.
			/// </summary>
			/// <param name="mi">Method</param>
			[ImportForwardCall(typeof(MethodDesc), nameof(MethodDesc.Reset), ImportCallOptions.Map)]
			internal static void Restore(MethodInfo mi)
			{
				Functions.Native.CallVoid((void*) Imports[nameof(Restore)], Runtime.ResolveHandle(mi).ToPointer());
			}

			/// <summary>
			/// Sets the entry point for the method represented by <paramref name="mi"/> to <paramref name="ptr"/>
			/// </summary>
			/// <param name="mi">Method</param>
			/// <param name="ptr">Function pointer</param>
			/// <returns><c>true</c> if the operation succeeded; <c>false</c> otherwise</returns>
			/// <exception cref="InvalidOperationException">The process is not 64-bit</exception>
			[ImportForwardCall(typeof(MethodDesc), nameof(MethodDesc.SetNativeCodeInterlocked), ImportCallOptions.Map)]
			internal static bool SetEntryPoint(MethodInfo mi, Pointer<byte> ptr)
			{
				if (!Mem.Is64Bit) {
					Guard.Fail();
				}

				Restore(mi);

				return Functions.Native.Call<bool>((void*) Imports[nameof(SetEntryPoint)],
				                                   mi.MethodHandle.Value.ToPointer(), ptr.ToPointer());
			}
		}
	}
}
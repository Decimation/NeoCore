using System;
using System.Runtime.InteropServices;
using NeoCore.Memory.Pointers;

namespace NeoCore.Interop
{
	public static partial class Functions
	{
		public static class Export
		{
			public static TDelegate Find<TDelegate>(Pointer<byte> h, string s) where TDelegate : Delegate
			{
				var f = Interop.Native.Kernel.GetProcAddress(h.Address, s);
				var fn = Marshal.GetDelegateForFunctionPointer<TDelegate>(f);
				return fn;
			}
			public static Delegate Find(Pointer<byte> h, string s, Type t)
			{
				var f  = Interop.Native.Kernel.GetProcAddress(h.Address, s);
				var fn = Marshal.GetDelegateForFunctionPointer(f,t);
				return fn;
			}
		}
	}
}
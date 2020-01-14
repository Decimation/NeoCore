using System;
using System.Runtime.InteropServices;
using Memkit.Pointers;

namespace NeoCore.Win32
{
	public static partial class Functions
	{
		public static class Export
		{
			public static TDelegate Find<TDelegate>(Pointer<byte> h, string s) where TDelegate : Delegate
			{
				var f = Win32.Native.Kernel.GetProcAddress(h.Address, s);
				var fn = Marshal.GetDelegateForFunctionPointer<TDelegate>(f);
				return fn;
			}

			public static Delegate Find(Pointer<byte> h, string s, Type t)
			{
				var f  = Win32.Native.Kernel.GetProcAddress(h.Address, s);
				var fn = Marshal.GetDelegateForFunctionPointer(f,t);
				return fn;
			}
		}
	}
}
using System;
using System.Runtime.InteropServices;

namespace NeoCore.Win32
{
	internal static partial class Native
	{
		/// <summary>
		/// General kernel functions from <see cref="KERNEL32_DLL"/>
		/// </summary>
		internal static class Kernel
		{
			[DllImport(KERNEL32_DLL, CharSet = CharSet.Auto, EntryPoint = nameof(GetModuleHandle))]
			internal static extern IntPtr GetModuleHandle(string moduleName);

			[DllImport(KERNEL32_DLL, CharSet = CharSet.Ansi, SetLastError = true, EntryPoint = nameof(GetProcAddress))]
			internal static extern IntPtr GetProcAddress(IntPtr module, string procName);

			[DllImport(KERNEL32_DLL, CharSet = CharSet.Ansi, SetLastError = true, EntryPoint = nameof(LoadLibrary))]
			internal static extern IntPtr LoadLibrary(string name);
		}
	}
}
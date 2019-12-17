using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using NeoCore.Interop.Structures;
using NeoCore.Memory;

namespace NeoCore.Interop
{
	using As = MarshalAsAttribute;
	using Types = UnmanagedType;

	internal static partial class Native
	{
		/// <summary>
		/// General kernel functions from <see cref="KERNEL32_DLL"/>
		/// </summary>
		internal static class Kernel
		{
			#region Abstraction

			internal static IntPtr OpenProcess(Process proc, ProcessAccess flags = ProcessAccess.All) =>
				OpenProcess(flags, false, proc.Id);

			internal static IntPtr OpenCurrentProcess(ProcessAccess flags = ProcessAccess.All) =>
				OpenProcess(Process.GetCurrentProcess(), flags);

			#endregion

			[DllImport(KERNEL32_DLL, SetLastError = true, PreserveSig = true, EntryPoint = nameof(CloseHandle))]
			internal static extern bool CloseHandle(IntPtr obj);

			[DllImport(KERNEL32_DLL, SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = nameof(OpenProcess))]
			private static extern IntPtr OpenProcess(ProcessAccess desiredAccess, bool inheritHandle, int processId);

			[DllImport(KERNEL32_DLL, SetLastError = true)]
			internal static extern IntPtr GetCurrentProcess();


			[DllImport(KERNEL32_DLL, EntryPoint = nameof(VirtualQuery))]
			internal static extern IntPtr VirtualQuery(IntPtr                     address,
			                                           ref MemoryInfo buffer, int length);

			[DllImport(KERNEL32_DLL, EntryPoint = nameof(VirtualProtect))]
			internal static extern bool VirtualProtect(IntPtr               address, int size,
			                                           MemoryProtection     newProtect,
			                                           out MemoryProtection oldProtect);


			[DllImport(KERNEL32_DLL, CharSet = CharSet.Auto, EntryPoint = nameof(GetModuleHandle))]
			internal static extern IntPtr GetModuleHandle(string moduleName);

			[DllImport(KERNEL32_DLL, CharSet = CharSet.Ansi, SetLastError = true, EntryPoint = nameof(GetProcAddress))]
			internal static extern IntPtr GetProcAddress(IntPtr module, string procName);


			#region Read / write

			[DllImport(KERNEL32_DLL, EntryPoint = nameof(Mem.Kernel.ReadProcessMemory))]
			internal static extern bool ReadProcMemoryInternal(IntPtr proc, IntPtr  baseAddr, IntPtr buffer,
			                                                   int    size, out int numBytesRead);


			[DllImport(KERNEL32_DLL, SetLastError = true, EntryPoint = nameof(Mem.Kernel.WriteProcessMemory))]
			internal static extern bool WriteProcMemoryInternal(IntPtr proc, IntPtr  baseAddr, IntPtr buffer,
			                                                    int    size, out int numberBytesWritten);

			#endregion
		}
	}
}
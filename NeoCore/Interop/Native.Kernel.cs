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
			internal static extern bool CloseHandle(IntPtr hObject);

			[DllImport(KERNEL32_DLL, SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = nameof(OpenProcess))]
			private static extern IntPtr OpenProcess(ProcessAccess dwDesiredAccess,
			                                         bool          bInheritHandle,
			                                         int           processId);

			[DllImport(KERNEL32_DLL, SetLastError = true)]
			internal static extern IntPtr GetCurrentProcess();


			[DllImport(KERNEL32_DLL, EntryPoint = nameof(VirtualQuery))]
			internal static extern IntPtr VirtualQuery(IntPtr                     address,
			                                           ref MemoryBasicInformation buffer,
			                                           int                        length);

			[DllImport(KERNEL32_DLL, EntryPoint = nameof(VirtualProtect))]
			internal static extern bool VirtualProtect(IntPtr               lpAddress,
			                                           int                  dwSize,
			                                           MemoryProtection     flNewProtect,
			                                           out MemoryProtection lpflOldProtect);


			[DllImport(KERNEL32_DLL, CharSet = CharSet.Auto, EntryPoint = nameof(GetModuleHandle))]
			internal static extern IntPtr GetModuleHandle(string lpModuleName);

			[DllImport(KERNEL32_DLL, CharSet = CharSet.Ansi, SetLastError = true, EntryPoint = nameof(GetProcAddress))]
			internal static extern IntPtr GetProcAddress(IntPtr hModule, string procName);


			#region Read / write

			[DllImport(KERNEL32_DLL, EntryPoint = nameof(Mem.Kernel.ReadProcessMemory))]
			internal static extern bool ReadProcessMemoryInternal(IntPtr  hProcess, IntPtr lpBaseAddress,
			                                                      IntPtr  lpBuffer, int    nSize,
			                                                      out int lpNumberOfBytesRead);


			[DllImport(KERNEL32_DLL, SetLastError = true, EntryPoint = nameof(Mem.Kernel.WriteProcessMemory))]
			internal static extern bool WriteProcessMemoryInternal(IntPtr  hProcess, IntPtr lpBaseAddress,
			                                                       IntPtr  lpBuffer, int    dwSize,
			                                                       out int lpNumberOfBytesWritten);

			#endregion
		}

		
	}
}
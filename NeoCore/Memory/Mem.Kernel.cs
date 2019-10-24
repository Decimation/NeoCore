using System;
using System.ComponentModel;
using System.Diagnostics;
using NeoCore.Interop;
using NeoCore.Memory.Pointers;

namespace NeoCore.Memory
{
	public static unsafe partial class Mem
	{
		public static class Kernel
		{
			#region Read / Write

			public static T ReadCurrentProcessMemory<T>(Pointer<byte> ptrBase) =>
				ReadProcessMemory<T>(Process.GetCurrentProcess(), ptrBase);

			public static T ReadProcessMemory<T>(Process proc, Pointer<byte> ptrBase)
			{
				T   t    = default;
				int size = Unsafe.SizeOf<T>();
				var ptr  = Unsafe.AddressOf(ref t);

				ReadProcessMemory(proc, ptrBase.Address, ptr.Address, size);

				return t;
			}

			public static void WriteCurrentProcessMemory<T>(Pointer<byte> ptrBase, T value) =>
				WriteProcessMemory(Process.GetCurrentProcess(), ptrBase, value);

			public static void WriteProcessMemory<T>(Process proc, Pointer<byte> ptrBase, T value)
			{
				int dwSize = Unsafe.SizeOf<T>();
				var ptr    = Unsafe.AddressOf(ref value);

				WriteProcessMemory(proc, ptrBase.Address, ptr.Address, dwSize);
			}

			#endregion

			#region Read / write raw bytes

			#region Read raw bytes

			public static void ReadProcessMemory(Process       proc,      Pointer<byte> ptrBase,
			                                     Pointer<byte> ptrBuffer, int           cb)
			{
				var hProc = Native.Kernel32.OpenProcess(proc);


				// Read the memory
				bool ok = (Native.Kernel32.ReadProcessMemoryInternal(hProc, ptrBase.Address,
				                                                     ptrBuffer.Address, cb,
				                                                     out int numberOfBytesRead));

				if (numberOfBytesRead != cb || !ok) {
					throw new Win32Exception();
				}

				// Close the handle
				Native.Kernel32.CloseHandle(hProc);
			}

			public static byte[] ReadProcessMemory(Process proc, Pointer<byte> ptrBase, int cb)
			{
				var mem = new byte[cb];

				fixed (byte* p = mem) {
					ReadProcessMemory(proc, ptrBase, (IntPtr) p, cb);
				}

				return mem;
			}


			#region Current process

			public static byte[] ReadCurrentProcessMemory(Pointer<byte> ptrBase, int cb)
			{
				return ReadProcessMemory(Process.GetCurrentProcess(), ptrBase, cb);
			}

			public static void ReadCurrentProcessMemory(Pointer<byte> ptrBase, Pointer<byte> ptrBuffer, int cb)
			{
				ReadProcessMemory(Process.GetCurrentProcess(), ptrBase, ptrBuffer, cb);
			}

			#endregion

			#endregion

			#region Write raw bytes

			#region Current process

			public static void WriteCurrentProcessMemory(Pointer<byte> ptrBase, byte[] value)
			{
				WriteProcessMemory(Process.GetCurrentProcess(), ptrBase, value);
			}

			public static void WriteCurrentProcessMemory(Pointer<byte> ptrBase, Pointer<byte> ptrBuffer,
			                                             int           dwSize)
			{
				WriteProcessMemory(Process.GetCurrentProcess(), ptrBase, ptrBuffer, dwSize);
			}

			#endregion

			public static void WriteProcessMemory(Process proc, Pointer<byte> ptrBase, Pointer<byte> ptrBuffer,
			                                      int     dwSize)
			{
				var hProc = Native.Kernel32.OpenProcess(proc);

				// Write the memory
				bool ok = (Native.Kernel32.WriteProcessMemoryInternal(hProc, ptrBase.Address, ptrBuffer.Address,
				                                                      dwSize, out int numberOfBytesWritten));


				if (numberOfBytesWritten != dwSize || !ok) {
					throw new Win32Exception();
				}


				// Close the handle
				Native.Kernel32.CloseHandle(hProc);
			}

			public static void WriteProcessMemory(Process proc, Pointer<byte> ptrBase, byte[] value)
			{
				fixed (byte* rg = value) {
					WriteProcessMemory(proc, ptrBase, (IntPtr) rg, value.Length);
				}
			}

			#endregion

			#endregion
		}
	}
}
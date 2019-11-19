using System;
using System.IO;
using System.Runtime.InteropServices;
using NeoCore.Utilities;

namespace NeoCore.Interop
{
	using As = MarshalAsAttribute;
	using Types = UnmanagedType;

	internal static partial class Native
	{
		/// <summary>
		/// File system functions from <see cref="KERNEL32_DLL"/>
		/// </summary>
		internal static class FileSystem
		{
			#region Abstraction

			private static IntPtr CreateFile(string   fileName, FileAccess     access, FileShare share,
			                                 FileMode mode,     FileAttributes attributes)
			{
				return CreateFile(fileName, access, share, IntPtr.Zero, mode, attributes, IntPtr.Zero);
			}

			private static void GetFileSize(string pFileName, out ulong fileSize)
			{
				var hFile = CreateFile(pFileName, FileAccess.Read, FileShare.Read, FileMode.Open, default);

				fileSize = GetFileSize(hFile, IntPtr.Zero);

				Kernel.CloseHandle(hFile);
			}

			internal static void GetFileParams(string pFileName, out ulong baseAddr, out ulong fileSize)
			{
				// Is it .PDB file ?

				if (pFileName.Contains(Format.PDB_EXT)) {
					// Yes, it is a .PDB file 

					// Determine its size, and use a dummy base address 

					// it can be any non-zero value, but if we load symbols 
					// from more than one file, memory regions specified
					// for different files should not overlap
					// (region is "base address + file size")
					baseAddr = 0x10000000;


					GetFileSize(pFileName, out fileSize);
				}
				else {
					// It is not a .PDB file 

					// Base address and file size can be 0 

					baseAddr = 0;
					fileSize = 0;

					throw new NotImplementedException();
				}
			}

			#endregion

			#region File native

			[DllImport(KERNEL32_DLL, SetLastError = true, CharSet = CharSet.Auto, EntryPoint = nameof(CreateFile))]
			private static extern IntPtr CreateFile(string         fileName,
			                                        FileAccess     fileAccess,
			                                        FileShare      fileShare,
			                                        IntPtr         securityAttributes,
			                                        FileMode       creationDisposition,
			                                        FileAttributes flagsAndAttributes,
			                                        IntPtr         template);

			[DllImport(KERNEL32_DLL, EntryPoint = nameof(GetFileSize))]
			private static extern uint GetFileSize(IntPtr hFile, IntPtr lpFileSizeHigh);

			#endregion
		}
	}
}
using System;
using System.IO;
using System.Runtime.InteropServices;
using Memkit.Interop;

namespace NeoCore.Win32
{
	public static class FileSystem
	{
		public static bool TryGetFullPath(string fileName, string cd, out string fullPath)
		{
			fullPath = GetFullPath(fileName, cd);
			return fullPath != null;
		}

		public static string GetFullPath(string fileName, string cd)
		{
			foreach (var path in cd.Split(Path.PathSeparator)) {
				var fullPath = Path.Combine(path, fileName);
				if (File.Exists(fullPath))
					return fullPath;
			}

			return null;
		}

		public static uint GetFileSize(IntPtr hFile) => GetFileSizeInternal(hFile, IntPtr.Zero);

		public static IntPtr CreateFile(string         fileName,
		                                FileAccess     access,
		                                FileShare      share,
		                                FileMode       mode,
		                                FileAttributes attributes)
		{
			return CreateFileInternal(fileName, access, share, IntPtr.Zero,
			                          mode, attributes, IntPtr.Zero);
		}

		[DllImport(Native.KERNEL32_DLL, SetLastError = true, CharSet = CharSet.Auto, EntryPoint = nameof(CreateFile))]
		private static extern IntPtr CreateFileInternal(string         fileName, FileAccess fileAccess,
		                                                FileShare      fileShare,
		                                                IntPtr         securityAttributes,
		                                                FileMode       creationDisposition,
		                                                FileAttributes flagsAndAttributes,
		                                                IntPtr         template);

		[DllImport(Native.KERNEL32_DLL, EntryPoint = nameof(GetFileSize))]
		private static extern uint GetFileSizeInternal(IntPtr hFile, IntPtr lpFileSizeHigh);
	}
}
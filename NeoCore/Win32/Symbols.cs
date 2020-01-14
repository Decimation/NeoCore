using System;
using System.Runtime.InteropServices;
using Memkit.Interop;
using NeoCore.Utilities.Diagnostics;
using NeoCore.Win32.Structures;

namespace NeoCore.Win32
{
	/// <summary>
	/// Symbol functions from <see cref="Native.DBGHELP_DLL"/>
	/// </summary>
	internal static class Symbols
	{
		private const string SYM_PREFIX = "Sym";


		[DllImport(Native.DBGHELP_DLL, EntryPoint = SYM_PREFIX + nameof(Initialize), CharSet = CharSet.Unicode)]
		private static extern bool Initialize(IntPtr hProcess, IntPtr userSearchPath, bool fInvadeProcess);


		[DllImport(Native.DBGHELP_DLL, EntryPoint = SYM_PREFIX + nameof(Cleanup), CharSet = CharSet.Unicode)]
		internal static extern bool Cleanup(IntPtr hProcess);


		[DllImport(Native.DBGHELP_DLL, EntryPoint = SYM_PREFIX + nameof(EnumSymbols), CharSet = CharSet.Unicode)]
		private static extern bool EnumSymbols(IntPtr hProcess, ulong               modBase,
		                                       string mask,     EnumSymbolsCallback callback,
		                                       IntPtr pUserContext);


		[DllImport(Native.DBGHELP_DLL, EntryPoint = SYM_PREFIX + nameof(GetOptions))]
		internal static extern SymbolOptions GetOptions();

		[DllImport(Native.DBGHELP_DLL, EntryPoint = SYM_PREFIX + nameof(SetOptions))]
		internal static extern SymbolOptions SetOptions(SymbolOptions options);


		[DllImport(Native.DBGHELP_DLL, EntryPoint = SYM_PREFIX + nameof(FromName))]
		private static extern bool FromName(IntPtr hProcess, string name, IntPtr pSymbol);


		[DllImport(Native.DBGHELP_DLL, EntryPoint = SYM_PREFIX + nameof(UnloadModule64))]
		internal static extern bool UnloadModule64(IntPtr hProc, ulong baseAddr);


		[DllImport(Native.DBGHELP_DLL, EntryPoint = SYM_PREFIX + nameof(LoadModuleEx), CharSet = CharSet.Unicode)]
		private static extern ulong LoadModuleEx(IntPtr hProcess,   IntPtr hFile,     string imageName,
		                                         string moduleName, ulong  baseOfDll, uint   dllSize,
		                                         IntPtr data,       uint   flags);

		internal delegate bool EnumSymbolsCallback(IntPtr symInfo, uint symbolSize, IntPtr pUserContext);


		#region Abstraction

		internal static void Initialize(IntPtr hProcess) =>
			Initialize(hProcess, IntPtr.Zero, false);


		internal static bool EnumSymbols(IntPtr hProcess, ulong modBase, EnumSymbolsCallback callback) =>
			EnumSymbols(hProcess, modBase, null, callback, IntPtr.Zero);

		internal static ulong LoadModuleEx(IntPtr hProc, string img, ulong dllBase, uint fileSize)
		{
			return LoadModuleEx(hProc, IntPtr.Zero, img, null, dllBase,
			                    fileSize, IntPtr.Zero, default);
		}


		internal static unsafe Symbol GetSymbol(IntPtr hProc, string name)
		{
			byte* byteBuffer = stackalloc byte[DebugSymbol.FullSize];
			var   buffer     = (DebugSymbol*) byteBuffer;

			buffer->SizeOfStruct = (uint) DebugSymbol.SizeOf;
			buffer->MaxNameLen   = DebugSymbol.MaxNameLength;

			Guard.Assert<NativeException>(FromName(hProc, name, (IntPtr) buffer),
			                              "Symbol \"{0}\" not found", name);

			return new Symbol(buffer);
		}

		#endregion
	}
}
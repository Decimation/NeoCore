using System;
using System.Runtime.InteropServices;
using System.Security;
using Microsoft.VisualBasic;
using NeoCore.CoreClr;
using NeoCore.Interop.Structures;
using NeoCore.Memory;
using NeoCore.Memory.Pointers;
using NeoCore.Utilities.Diagnostics;

namespace NeoCore.Interop
{
	using As = MarshalAsAttribute;
	using Types = UnmanagedType;

	internal static unsafe partial class Native
	{
		/// <summary>
		/// Symbol functions from <see cref="DBGHELP_DLL"/>
		/// </summary>
		internal static class Symbols
		{
			private const string SYM_PREFIX = "Sym";


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


			internal static Symbol GetSymbol(IntPtr hProc, string name)
			{
				byte* byteBuffer = stackalloc byte[NativeSymbol.FullSize];
				var   buffer     = (NativeSymbol*) byteBuffer;

				buffer->SizeOfStruct = (uint) NativeSymbol.SizeOf;
				buffer->MaxNameLen   = NativeSymbol.MaxNameLength;

				Guard.Assert<NativeException>(FromName(hProc, name, (IntPtr) buffer),
				                              "Symbol \"{0}\" not found", name);

				return new Symbol(buffer);
			}

			#endregion

			internal delegate bool EnumSymbolsCallback(IntPtr symInfo, uint symbolSize, IntPtr pUserContext);


			[DllImport(DBGHELP_DLL, EntryPoint = SYM_PREFIX + nameof(Initialize), CharSet = CharSet.Unicode)]
			private static extern bool Initialize(IntPtr hProcess,
			                                      IntPtr userSearchPath,
			                                      bool   fInvadeProcess);


			[DllImport(DBGHELP_DLL, EntryPoint = SYM_PREFIX + nameof(Cleanup), CharSet = CharSet.Unicode)]
			internal static extern bool Cleanup(IntPtr hProcess);


			[DllImport(DBGHELP_DLL, EntryPoint = SYM_PREFIX + nameof(EnumSymbols), CharSet = CharSet.Unicode)]
			private static extern bool EnumSymbols(IntPtr hProcess, ulong               modBase,
			                                       string mask,     EnumSymbolsCallback callback,
			                                       IntPtr pUserContext);


			[DllImport(DBGHELP_DLL, EntryPoint = SYM_PREFIX + nameof(GetOptions))]
			internal static extern SymbolOptions GetOptions();

			[DllImport(DBGHELP_DLL, EntryPoint = SYM_PREFIX + nameof(SetOptions))]
			internal static extern SymbolOptions SetOptions(SymbolOptions options);


			[DllImport(DBGHELP_DLL, EntryPoint = SYM_PREFIX + nameof(FromName))]
			private static extern bool FromName(IntPtr hProcess, string name, IntPtr pSymbol);


			[DllImport(DBGHELP_DLL, EntryPoint = SYM_PREFIX + nameof(UnloadModule64))]
			internal static extern bool UnloadModule64(IntPtr hProc, ulong baseAddr);


			[DllImport(DBGHELP_DLL, EntryPoint = SYM_PREFIX + nameof(LoadModuleEx), CharSet = CharSet.Unicode)]
			private static extern ulong LoadModuleEx(IntPtr hProcess,   IntPtr hFile,     string imageName,
			                                         string moduleName, ulong  baseOfDll, uint   dllSize,
			                                         IntPtr data,       uint   flags);
		}
	}
}
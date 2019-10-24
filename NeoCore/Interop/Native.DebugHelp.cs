using System;
using System.Runtime.InteropServices;
using System.Security;
using NeoCore.CoreClr;
using NeoCore.Interop.Enums;
using NeoCore.Interop.Structures;
using NeoCore.Memory;
using NeoCore.Utilities.Diagnostics;
using static NeoCore.Assets.Constants;
using static NeoCore.Assets.Constants.Native;

namespace NeoCore.Interop
{
	using As = MarshalAsAttribute;
	using Types = UnmanagedType;

	internal static unsafe partial class Native
	{
		/// <summary>
		/// Functions from <see cref="DBGHELP_DLL"/>
		/// </summary>
		internal static class DebugHelp
		{
			//todo: clean up

			/// <summary>
			///     https://github.com/Microsoft/DbgShell/blob/master/DbgProvider/internal/Native/DbgHelp.cs
			/// </summary>
			private const string DBGHELP_DLL = "DbgHelp.dll";


			#region Abstraction

			internal static void SymInitialize(IntPtr hProcess)
			{
				SymInitialize(hProcess, IntPtr.Zero, false);
			}


			internal static bool SymEnumSymbols(IntPtr hProcess, ulong modBase, SymEnumSymbolsCallback callback)
			{
				return SymEnumSymbols(hProcess, modBase, null, callback, IntPtr.Zero);
			}

			internal static ulong SymLoadModuleEx(IntPtr hProc, string img, ulong dllBase, uint fileSize)
			{
				return SymLoadModuleEx(hProc, IntPtr.Zero, img, null, dllBase,
				                       fileSize, IntPtr.Zero, default);
			}


			internal static string GetSymbolName(IntPtr sym)
			{
				var pSym = (DebugSymbol*) sym;
				return Mem.ReadString(&pSym->Name, (int) pSym->NameLen);
			}

			internal static Symbol GetSymbol(IntPtr hProc, string name)
			{
				var sz = (int) (StructureSize + MaxSymbolNameLength * sizeof(byte)
				                              + sizeof(ulong) - 1 / sizeof(ulong));

				byte* byteBuffer = stackalloc byte[sz];
				var   buffer     = (DebugSymbol*) byteBuffer;

				buffer->SizeOfStruct = (uint) StructureSize;
				buffer->MaxNameLen   = MaxSymbolNameLength;


				if (SymFromName(hProc, name, (IntPtr) buffer)) {
					var firstChar = &buffer->Name;
					var symName   = Mem.ReadString(firstChar, (int) buffer->NameLen);
					return new Symbol(buffer, symName);
				}

				throw new NativeException(String.Format("Symbol \"{0}\" not found", name));
			}

			#endregion

			#region Sym misc

			[SuppressUnmanagedCodeSecurity]
			[DefaultDllImportSearchPaths(DllImportSearchPath.LegacyBehavior)]
			[DllImport(DBGHELP_DLL, SetLastError = true, CharSet = CharSet.Unicode)]
			[return: As(Types.Bool)]
			private static extern bool SymInitialize(IntPtr                hProcess,
			                                         IntPtr                userSearchPath,
			                                         [As(Types.Bool)] bool fInvadeProcess);


			[SuppressUnmanagedCodeSecurity]
			[DefaultDllImportSearchPaths(DllImportSearchPath.LegacyBehavior)]
			[DllImport(DBGHELP_DLL, SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = nameof(SymCleanup))]
			[return: As(Types.Bool)]
			internal static extern bool SymCleanup(IntPtr hProcess);

			/// <summary>
			///     SYM_ENUMERATESYMBOLS_CALLBACK
			/// </summary>
			/// <param name="symInfo">SYMBOL_INFO</param>
			[return: As(Types.Bool)]
			internal delegate bool SymEnumSymbolsCallback(IntPtr symInfo, uint symbolSize, IntPtr pUserContext);

			#endregion

			#region Sym enum types

			[DefaultDllImportSearchPaths(DllImportSearchPath.LegacyBehavior)]
			[DllImport(DBGHELP_DLL, SetLastError = true, CharSet = CharSet.Unicode)]
			[return: As(Types.Bool)]
			private static extern bool SymEnumTypesByName(IntPtr                 hProcess,
			                                              ulong                  modBase,
			                                              string                 mask,
			                                              SymEnumSymbolsCallback callback,
			                                              IntPtr                 pUserContext);

			[SuppressUnmanagedCodeSecurity]
			[DefaultDllImportSearchPaths(DllImportSearchPath.LegacyBehavior)]
			[DllImport(DBGHELP_DLL, SetLastError = true, CharSet = CharSet.Unicode)]
			[return: As(Types.Bool)]
			private static extern bool SymEnumTypes(IntPtr                 hProcess, ulong  modBase,
			                                        SymEnumSymbolsCallback callback, IntPtr pUserContext);

			#endregion

			#region Sym enum symbols

			/// <param name="hProcess">Process handle of the current process</param>
			/// <param name="modBase">Base address of the module</param>
			/// <param name="mask">Mask (NULL -> all symbols)</param>
			/// <param name="callback">The callback function</param>
			/// <param name="pUserContext">A used-defined context can be passed here, if necessary</param>
			[DefaultDllImportSearchPaths(DllImportSearchPath.LegacyBehavior)]
			[DllImport(DBGHELP_DLL, SetLastError = true, CharSet = CharSet.Unicode)]
			[return: As(Types.Bool)]
			internal static extern bool SymEnumSymbols(IntPtr hProcess, ulong                  modBase,
			                                           string mask,     SymEnumSymbolsCallback callback,
			                                           IntPtr pUserContext);

			#endregion

			#region Sym options

			[DllImport(DBGHELP_DLL)]
			internal static extern SymbolOptions SymGetOptions();

			[DllImport(DBGHELP_DLL)]
			internal static extern SymbolOptions SymSetOptions(SymbolOptions options);

			#endregion

			#region Sym from

			[DllImport(DBGHELP_DLL)]
			[return: As(Types.Bool)]
			private static extern bool SymFromName(IntPtr hProcess, IntPtr name, IntPtr pSymbol);

			[DllImport(DBGHELP_DLL)]
			[SuppressUnmanagedCodeSecurity]
			[return: As(Types.Bool)]
			private static extern bool SymFromName(IntPtr hProcess, string name, IntPtr pSymbol);


			[DllImport(DBGHELP_DLL, SetLastError = true)]
			[return: As(Types.Bool)]
			private static extern bool SymFromAddr(IntPtr hProc, ulong addr, ulong* displacement, DebugSymbol* pSym);

			#endregion

			#region Sym module

			[DllImport(DBGHELP_DLL)]
			private static extern ulong SymLoadModule64(IntPtr hProc, IntPtr h, string p, string s, ulong baseAddr,
			                                            uint   fileSize);


			[DllImport(DBGHELP_DLL, SetLastError = true)]
			private static extern bool SymGetModuleInfo64(IntPtr hProc, ulong qwAddr, IntPtr pModInfo);


			[DllImport(DBGHELP_DLL)]
			[return: As(Types.Bool)]
			internal static extern bool SymUnloadModule64(IntPtr hProc, ulong baseAddr);


			/// <param name="hProcess">Process handle of the current process</param>
			/// <param name="hFile">Handle to the module's image file (not needed)</param>
			/// <param name="imageName">Path/name of the file</param>
			/// <param name="moduleName">User-defined short name of the module (it can be NULL)</param>
			/// <param name="baseOfDll">Base address of the module (cannot be NULL if .PDB file is used, otherwise it can be NULL)</param>
			/// <param name="dllSize">Size of the file (cannot be NULL if .PDB file is used, otherwise it can be NULL)</param>
			/// <param name="data">?</param>
			/// <param name="flags">?</param>
			[DefaultDllImportSearchPaths(DllImportSearchPath.LegacyBehavior)]
			[DllImport(DBGHELP_DLL, SetLastError = true, CharSet = CharSet.Unicode)]
			private static extern ulong SymLoadModuleEx(IntPtr hProcess,   IntPtr hFile,     string imageName,
			                                            string moduleName, ulong  baseOfDll, uint   dllSize,
			                                            IntPtr data,       uint   flags);

			#endregion
		}
	}
}
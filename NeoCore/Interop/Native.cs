using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace NeoCore.Interop
{
	internal static partial class Native
	{
		/// <summary>
		///     https://github.com/Microsoft/DbgShell/blob/master/DbgProvider/internal/Native/DbgHelp.cs
		/// </summary>
		private const string DBGHELP_DLL = "DbgHelp.dll";

		private const string KERNEL32_DLL = "kernel32.dll";

		internal const string DLL_EXT = ".dll";

		internal const string PDB_EXT = ".pdb";
	}

	/**
	 * Enums that do not have an accompanying structure will be placed here
	 */

	[Flags]
	public enum ProcessAccess : uint
	{
		All                     = 0x1F0FFF,
		Terminate               = 0x000001,
		CreateThread            = 0x000002,
		VmOperation             = 0x00000008,
		VmRead                  = 0x000010,
		VmWrite                 = 0x000020,
		DupHandle               = 0x00000040,
		CreateProcess           = 0x000080,
		SetInformation          = 0x00000200,
		QueryInformation        = 0x000400,
		QueryLimitedInformation = 0x001000,
		Synchronize             = 0x00100000
	}

	public enum MachineArchitecture : ushort
	{
		/// <summary>
		/// x86
		/// </summary>
		I386 = 0x014C,

		/// <summary>
		/// Intel Itanium
		/// </summary>
		IA64 = 0x0200,

		/// <summary>
		/// x64
		/// </summary>
		AMD64 = 0x8664
	}

	[Flags]
	public enum ConsoleMode : ushort
	{
		ENABLE_ECHO_INPUT             = 0x0004,
		ENABLE_EXTENDED_FLAGS         = 0x0080,
		ENABLE_INSERT_MODE            = 0x0020,
		ENABLE_LINE_INPUT             = 0x0002,
		ENABLE_MOUSE_INPUT            = 0x0010,
		ENABLE_PROCESSED_INPUT        = 0x0001,
		ENABLE_QUICK_EDIT_MODE        = 0x0040,
		ENABLE_WINDOW_INPUT           = 0x0008,
		ENABLE_VIRTUAL_TERMINAL_INPUT = 0x0200,
	}

	public enum HandleOption : int
	{
		STD_INPUT_HANDLE  = -10,
		STD_OUTPUT_HANDLE = -11,
		STD_ERROR_HANDLE  = -12
	}

	[Flags]
	public enum OutputMode : ushort
	{
		ENABLE_PROCESSED_OUTPUT            = 0x0001,
		ENABLE_WRAP_AT_EOL_OUTPUT          = 0x0002,
		ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004,
		DISABLE_NEWLINE_AUTO_RETURN        = 0x0008,
		ENABLE_LVB_GRID_WORLDWIDE          = 0x0010,
	}
}
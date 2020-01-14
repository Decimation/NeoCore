using System;

// ReSharper disable InconsistentNaming

namespace NeoCore.Win32
{
	internal static partial class Native
	{
		/// <summary>
		///     https://github.com/Microsoft/DbgShell/blob/master/DbgProvider/internal/Native/DbgHelp.cs
		/// </summary>
		private const string DBGHELP_DLL = "DbgHelp.dll";

		private const string KERNEL32_DLL = "Kernel32.dll";

		internal const string DLL_EXT = ".dll";

		internal const string PDB_EXT = ".pdb";

		internal const string CMD_EXE = "cmd.exe";

		internal const string SYMCHK_EXE = "symchk";
	}

	
	// Enums that do not have an accompanying structure will be placed here
	 


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
		EnableEchoInput             = 0x0004,
		EnableExtendedFlags         = 0x0080,
		EnableInsertMode            = 0x0020,
		EnableLineInput             = 0x0002,
		EnableMouseInput            = 0x0010,
		EnableProcessedInput        = 0x0001,
		EnableQuickEditMode        = 0x0040,
		EnableWindowInput           = 0x0008,
		EnableVirtualTerminalInput = 0x0200,
	}

	public enum HandleOption
	{
		StdInputHandle  = -10,
		StdOutputHandle = -11,
		StdErrorHandle  = -12
	}

	[Flags]
	public enum OutputMode : ushort
	{
		EnableProcessedOutput            = 0x0001,
		EnableWrapAtEOLOutput          = 0x0002,
		EnableVirtualTerminalProcessing = 0x0004,
		DisableNewlineAutoReturn        = 0x0008,
		EnableLVBGridWorldwide          = 0x0010,
	}
}
using System;

namespace NeoCore.Interop.Enums
{
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
}
using System;

// ReSharper disable InconsistentNaming

namespace NeoCore.Interop.Enums
{
	[Flags]
	public enum ImageSectionFlags : uint
	{
		/// <summary>
		/// Reserved for future use.
		/// </summary>
		TypeReg = 0x00000000,

		/// <summary>
		/// Reserved for future use.
		/// </summary>
		TypeDsect = 0x00000001,

		/// <summary>
		/// Reserved for future use.
		/// </summary>
		TypeNoLoad = 0x00000002,

		/// <summary>
		/// Reserved for future use.
		/// </summary>
		TypeGroup = 0x00000004,

		/// <summary>
		/// The section should not be padded to the next boundary. This flag is obsolete and is replaced by IMAGE_SCN_ALIGN_1BYTES. This is valid only for object files.
		/// </summary>
		TypeNoPadded = 0x00000008,

		/// <summary>
		/// Reserved for future use.
		/// </summary>
		TypeCopy = 0x00000010,

		/// <summary>
		/// The section contains executable code.
		/// </summary>
		ContentCode = 0x00000020,

		/// <summary>
		/// The section contains initialized data.
		/// </summary>
		ContentInitializedData = 0x00000040,

		/// <summary>
		/// The section contains uninitialized data.
		/// </summary>
		ContentUninitializedData = 0x00000080,

		/// <summary>
		/// Reserved for future use.
		/// </summary>
		LinkOther = 0x00000100,

		/// <summary>
		/// The section contains comments or other information. The .drectve section has this type. This is valid for object files only.
		/// </summary>
		LinkInfo = 0x00000200,

		/// <summary>
		/// Reserved for future use.
		/// </summary>
		TypeOver = 0x00000400,

		/// <summary>
		/// The section will not become part of the image. This is valid only for object files.
		/// </summary>
		LinkRemove = 0x00000800,

		/// <summary>
		/// The section contains COMDAT data. For more information, see section 5.5.6, COMDAT Sections (Object Only). This is valid only for object files.
		/// </summary>
		LinkComDat = 0x00001000,

		/// <summary>
		/// Reset speculative exceptions handling bits in the TLB entries for this section.
		/// </summary>
		NoDeferSpecExceptions = 0x00004000,

		/// <summary>
		/// The section contains data referenced through the global pointer (GP).
		/// </summary>
		RelativeGP = 0x00008000,

		/// <summary>
		/// Reserved for future use.
		/// </summary>
		MemPurgeable = 0x00020000,

		/// <summary>
		/// Reserved for future use.
		/// </summary>
		Memory16Bit = 0x00020000,

		/// <summary>
		/// Reserved for future use.
		/// </summary>
		MemoryLocked = 0x00040000,

		/// <summary>
		/// Reserved for future use.
		/// </summary>
		MemoryPreload = 0x00080000,

		/// <summary>
		/// Align data on a 1-byte boundary. Valid only for object files.
		/// </summary>
		Align1Bytes = 0x00100000,

		/// <summary>
		/// Align data on a 2-byte boundary. Valid only for object files.
		/// </summary>
		Align2Bytes = 0x00200000,

		/// <summary>
		/// Align data on a 4-byte boundary. Valid only for object files.
		/// </summary>
		Align4Bytes = 0x00300000,

		/// <summary>
		/// Align data on an 8-byte boundary. Valid only for object files.
		/// </summary>
		Align8Bytes = 0x00400000,

		/// <summary>
		/// Align data on a 16-byte boundary. Valid only for object files.
		/// </summary>
		Align16Bytes = 0x00500000,

		/// <summary>
		/// Align data on a 32-byte boundary. Valid only for object files.
		/// </summary>
		Align32Bytes = 0x00600000,

		/// <summary>
		/// Align data on a 64-byte boundary. Valid only for object files.
		/// </summary>
		Align64Bytes = 0x00700000,

		/// <summary>
		/// Align data on a 128-byte boundary. Valid only for object files.
		/// </summary>
		Align128Bytes = 0x00800000,

		/// <summary>
		/// Align data on a 256-byte boundary. Valid only for object files.
		/// </summary>
		Align256Bytes = 0x00900000,

		/// <summary>
		/// Align data on a 512-byte boundary. Valid only for object files.
		/// </summary>
		Align512Bytes = 0x00A00000,

		/// <summary>
		/// Align data on a 1024-byte boundary. Valid only for object files.
		/// </summary>
		Align1024Bytes = 0x00B00000,

		/// <summary>
		/// Align data on a 2048-byte boundary. Valid only for object files.
		/// </summary>
		Align2048Bytes = 0x00C00000,

		/// <summary>
		/// Align data on a 4096-byte boundary. Valid only for object files.
		/// </summary>
		Align4096Bytes = 0x00D00000,

		/// <summary>
		/// Align data on an 8192-byte boundary. Valid only for object files.
		/// </summary>
		Align8192Bytes = 0x00E00000,

		/// <summary>
		/// The section contains extended relocations.
		/// </summary>
		LinkExtendedRelocationOverflow = 0x01000000,

		/// <summary>
		/// The section can be discarded as needed.
		/// </summary>
		MemoryDiscardable = 0x02000000,

		/// <summary>
		/// The section cannot be cached.
		/// </summary>
		MemoryNotCached = 0x04000000,

		/// <summary>
		/// The section is not pageable.
		/// </summary>
		MemoryNotPaged = 0x08000000,

		/// <summary>
		/// The section can be shared in memory.
		/// </summary>
		MemoryShared = 0x10000000,

		/// <summary>
		/// The section can be executed as code.
		/// </summary>
		MemoryExecute = 0x20000000,

		/// <summary>
		/// The section can be read.
		/// </summary>
		MemoryRead = 0x40000000,

		/// <summary>
		/// The section can be written to.
		/// </summary>
		MemoryWrite = 0x80000000
	}

	public enum ImageDllCharacteristics : ushort
	{
		DYNAMIC_BASE          = 0x0040,
		FORCE_INTEGRITY       = 0x0080,
		NX_COMPAT             = 0x0100,
		NO_ISOLATION          = 0x0200,
		NO_SEH                = 0x0400,
		NO_BIND               = 0x0800,
		WDM_DRIVER            = 0x2000,
		TERMINAL_SERVER_AWARE = 0x8000
	}


	public enum ImageSubSystem : ushort
	{
		UNKNOWN                  = 0,
		NATIVE                   = 1,
		WINDOWS_GUID             = 2,
		WINDOWS_CUI              = 3,
		OS2_CUI                  = 5,
		POSIX_CUI                = 7,
		WINDOWS_CE_GUI           = 9,
		EFI_APPLICATION          = 10,
		EFI_BOOT_SERVICE_DRIVER  = 11,
		EFI_RUNTIME_DRIVER       = 12,
		EFI_ROM                  = 13,
		XBOX                     = 14,
		WINDOWS_BOOT_APPLICATION = 16
	}

	[Flags]
	public enum ImageFileCharacteristics : ushort
	{
		RELOCS_STRIPPED         = 0x0001,
		EXECUTABLE_IMAGE        = 0x0002,
		LINE_NUMS_STRIPPED      = 0x0004,
		LOCAL_SYMS_STRIPPED     = 0x0008,
		AGGRESSIVE_WS_TRIM      = 0x0010,
		LARGE_ADDRESS_AWARE     = 0x0020,
		BYTES_RESERVED_IO       = 0x0080,
		BIT32_MACHINE           = 0x0100,
		DEBUG_STRIPPED          = 0x0200,
		REMOVABLE_RUN_FROM_SWAP = 0x0400,
		NET_RUN_FROM_SWAP       = 0x0800,
		SYSTEM                  = 0x1000,
		DLL                     = 0x2000,
		UP_SYSTEM_ONLY          = 0x4000,
		BYTES_RESERVED_HI       = 0x8000
	}
}
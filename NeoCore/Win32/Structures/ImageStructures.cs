using System;
using System.Runtime.InteropServices;
using Memkit.Pointers;
using NeoCore.Model;
using NeoCore.Win32.Attributes;

// ReSharper disable InconsistentNaming


namespace NeoCore.Win32.Structures
{
	[NativeStructure]
	public unsafe struct ImageDOSHeader : INativeStructure
	{
		public string NativeName => "IMAGE_DOS_HEADER";

		// DOS .EXE header

		/// <summary>
		/// Magic number
		/// </summary>
		public ushort Magic { get; }

		/// <summary>
		/// Bytes on last page of file
		/// </summary>
		public ushort Cblp { get; }

		/// <summary>
		/// Pages in file
		/// </summary>
		public ushort Cp { get; }

		/// <summary>
		/// Relocations
		/// </summary>
		public ushort Crlc { get; }

		/// <summary>
		/// Size of header in paragraphs
		/// </summary>
		public ushort Cparhdr { get; }


		/// <summary>
		/// Minimum extra paragraphs needed
		/// </summary>
		public ushort Minalloc { get; }

		/// <summary>
		/// Maximum extra paragraphs needed
		/// </summary>
		public ushort Maxalloc { get; }

		/// <summary>
		/// Initial (relative) SS value
		/// </summary>
		public ushort Ss { get; }

		/// <summary>
		/// Initial SP value
		/// </summary>
		public ushort Sp { get; }

		/// <summary>
		/// Checksum
		/// </summary>
		public ushort Csum { get; }

		/// <summary>
		/// Initial IP value
		/// </summary>
		public ushort Ip { get; }

		/// <summary>
		/// Initial (relative) CS value
		/// </summary>
		public ushort Cs { get; }

		/// <summary>
		/// File address of relocation table
		/// </summary>
		public ushort Lfarlc { get; }

		/// <summary>
		/// Overlay number
		/// </summary>
		public ushort Ovno { get; }

		/// <summary>
		/// Reserved
		/// </summary>
		public fixed ushort Res0[4];

		/// <summary>
		/// OEM identifier (for <see cref="Oeminfo"/>)
		/// </summary>
		public ushort Oemid { get; }

		/// <summary>
		/// OM information; <see cref="Oemid"/> specific
		/// </summary>
		public ushort Oeminfo { get; }

		/// <summary>
		/// Reserved
		/// </summary>
		public fixed ushort Res2[10];

		/// <summary>
		/// File address of new exe header
		/// </summary>
		public uint Lfanew { get; }
	}

	[NativeStructure]
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct ImageFileHeader : INativeStructure
	{
		public string NativeName => "IMAGE_FILE_HEADER";

		public MachineArchitecture Machine { get; }

		public ushort NumberOfSections { get; }

		public uint TimeDateStamp { get; }

		public uint PointerToSymbolTable { get; }

		public uint NumberOfSymbols { get; }

		public ushort SizeOfOptionalHeader { get; }

		public ImageFileCharacteristics Characteristics { get; }
	}

	[NativeStructure]
	[StructLayout(LayoutKind.Sequential)]
	public struct ImageDataDirectory : INativeStructure
	{
		public string NativeName => "IMAGE_DATA_DIRECTORY";

		public uint VirtualAddress { get; }
		public uint Size           { get; }
	}

	[NativeStructure]
	[StructLayout(LayoutKind.Explicit)]
	public struct ImageSectionHeader : INativeStructure
	{
		public string NativeName => "IMAGE_SECTION_HEADER";

		// Grabbed the following 2 definitions from http://www.pinvoke.net/default.aspx/Structures/IMAGE_SECTION_HEADER.html


		[FieldOffset(0)]
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
		public char[] Name;

		public string SectionName => new string(Name);


		[FieldOffset(8)]
		public uint VirtualSize;


		[FieldOffset(12)]
		public uint VirtualAddress;


		[FieldOffset(16)]
		public uint SizeOfRawData;


		[FieldOffset(20)]
		public uint PointerToRawData;


		[FieldOffset(24)]
		public uint PointerToRelocations;


		[FieldOffset(28)]
		public uint PointerToLinenumbers;


		[FieldOffset(32)]
		public ushort NumberOfRelocations;


		[FieldOffset(34)]
		public ushort NumberOfLinenumbers;


		[FieldOffset(36)]
		public ImageSectionFlags Characteristics;
	}


	[NativeStructure]
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct ImageOptionalHeader64 : INativeStructure
	{
		public string NativeName => "IMAGE_OPTIONAL_HEADER64";

		public ushort Magic;

		public byte MajorLinkerVersion;

		public byte MinorLinkerVersion;

		public uint SizeOfCode;

		public uint SizeOfInitializedData;

		public uint SizeOfUninitializedData;

		public uint AddressOfEntryPoint;

		public uint BaseOfCode;

		public ulong ImageBase;

		public uint SectionAlignment;

		public uint FileAlignment;

		public ushort MajorOperatingSystemVersion;

		public ushort MinorOperatingSystemVersion;

		public ushort MajorImageVersion;

		public ushort MinorImageVersion;

		public ushort MajorSubsystemVersion;

		public ushort MinorSubsystemVersion;

		public uint Win32VersionValue;

		public uint SizeOfImage;

		public uint SizeOfHeaders;

		public uint CheckSum;

		public ImageSubSystem Subsystem;

		public ImageDllCharacteristics DllCharacteristics;

		public ulong SizeOfStackReserve;

		public ulong SizeOfStackCommit;

		public ulong SizeOfHeapReserve;

		public ulong SizeOfHeapCommit;

		public uint LoaderFlags;

		public uint NumberOfRvaAndSizes;

		public ImageDataDirectory ExportTable;

		public ImageDataDirectory ImportTable;

		public ImageDataDirectory ResourceTable;

		public ImageDataDirectory ExceptionTable;

		public ImageDataDirectory CertificateTable;

		public ImageDataDirectory BaseRelocationTable;

		public ImageDataDirectory Debug;

		public ImageDataDirectory Architecture;

		public ImageDataDirectory GlobalPtr;

		public ImageDataDirectory TLSTable;

		public ImageDataDirectory LoadConfigTable;

		public ImageDataDirectory BoundImport;

		public ImageDataDirectory IAT;

		public ImageDataDirectory DelayImportDescriptor;

		public ImageDataDirectory CLRRuntimeHeader;

		public ImageDataDirectory Reserved;
	}

	[NativeStructure]
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct ImageOptionalHeader32 : INativeStructure
	{
		public string NativeName => "IMAGE_OPTIONAL_HEADER32";

		public ushort Magic;

		public byte MajorLinkerVersion;

		public byte MinorLinkerVersion;

		public uint SizeOfCode;

		public uint SizeOfInitializedData;

		public uint SizeOfUninitializedData;

		public uint AddressOfEntryPoint;

		public uint BaseOfCode;

		public uint BaseOfData;

		public uint ImageBase;

		public uint SectionAlignment;

		public uint FileAlignment;

		public ushort MajorOperatingSystemVersion;

		public ushort MinorOperatingSystemVersion;

		public ushort MajorImageVersion;

		public ushort MinorImageVersion;

		public ushort MajorSubsystemVersion;

		public ushort MinorSubsystemVersion;

		public uint Win32VersionValue;

		public uint SizeOfImage;

		public uint SizeOfHeaders;

		public uint CheckSum;

		public ImageSubSystem Subsystem;

		public ImageDllCharacteristics DllCharacteristics;

		public uint SizeOfStackReserve;

		public uint SizeOfStackCommit;

		public uint SizeOfHeapReserve;

		public uint SizeOfHeapCommit;

		public uint LoaderFlags;

		public uint NumberOfRvaAndSizes;

		public ImageDataDirectory ExportTable;

		public ImageDataDirectory ImportTable;

		public ImageDataDirectory ResourceTable;

		public ImageDataDirectory ExceptionTable;

		public ImageDataDirectory CertificateTable;

		public ImageDataDirectory BaseRelocationTable;

		public ImageDataDirectory Debug;

		public ImageDataDirectory Architecture;

		public ImageDataDirectory GlobalPtr;

		public ImageDataDirectory TLSTable;

		public ImageDataDirectory LoadConfigTable;

		public ImageDataDirectory BoundImport;

		public ImageDataDirectory IAT;

		public ImageDataDirectory DelayImportDescriptor;

		public ImageDataDirectory CLRRuntimeHeader;

		public ImageDataDirectory Reserved;
	}

	[NativeStructure]
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct ImageNtHeaders64 : INativeStructure
	{
		public string NativeName => "IMAGE_NT_HEADERS64";

		public uint Signature { get; }

		public ImageFileHeader FileHeader { get; }

		public ImageOptionalHeader64 OptionalHeader { get; }
	}

	/// <summary>
	/// Wraps an <see cref="ImageSectionHeader"/>
	/// </summary>
	public sealed class ImageSectionInfo : IWrapper<ImageSectionHeader>
	{
		internal ImageSectionInfo(ImageSectionHeader struc, int number, IntPtr address)
		{
			Number          = number;
			Name            = new string(struc.Name);
			Address         = address;
			Size            = (int) struc.VirtualSize;
			Characteristics = struc.Characteristics;
		}

		public string Name { get; }

		public int Number { get; }

		public Pointer<byte> Address { get; }

		public int Size { get; }

		public ImageSectionFlags Characteristics { get; }

		public override string ToString()
		{
			return String.Format("Number: {0} | Name: {1} | Address: {2} | Size: {3} | Characteristics: {4}",
			                     Number, Name, Address, Size, Characteristics);
		}
	}

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

	[Flags]
	public enum ImageDllCharacteristics : ushort
	{
		DynamicBase         = 0x0040,
		ForceIntegrity      = 0x0080,
		NXCompat            = 0x0100,
		NoIsolation         = 0x0200,
		NoSEH               = 0x0400,
		NoBind              = 0x0800,
		WDMDriver           = 0x2000,
		TerminalServerAware = 0x8000
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
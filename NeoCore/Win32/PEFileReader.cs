using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Memkit.Interop;
using NeoCore.Utilities;
using NeoCore.Utilities.Extensions;
using NeoCore.Win32.Structures;

// ReSharper disable UnusedAutoPropertyAccessor.Global

// ReSharper disable InconsistentNaming

namespace NeoCore.Win32
{
	// Credits: John Stewien
	// From: http://code.cheesydesign.com/?p=572

	// How it works is that it first reads in the old DOS header,
	// at the end of this header is a file offset to the new NT File Header structure.
	// I seek to that position, and read in the NT File Header. From that header I can get the linker time stamp.
	// As this is a general purpose library I also check whether the header is 32 or 64 bit, and read in either
	// the Optional 32 bit Header, or the Optional 64 bit Header, which can then be used however you like.
	// This is how you would get the 32 bit header:

	/// <summary>
	/// Reads in the header information of the Portable Executable format.
	/// Provides information such as the date the assembly was compiled.
	/// </summary>
	public sealed class PEFileReader
	{
		#region Public Methods

		public PEFileReader(string filePath)
		{
			// Read in the DLL or EXE and get the timestamp
			using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
			using var reader = new BinaryReader(stream);

			DOSHeader = reader.ReadStructure<ImageDOSHeader>();

			// Add 4 bytes to the offset
			stream.Seek(DOSHeader.Lfanew, SeekOrigin.Begin);

			// NT Headers signature
			reader.ReadUInt32();

			FileHeader = reader.ReadStructure<ImageFileHeader>();

			if (Is32BitHeader) {
				OptionalHeader32 = reader.ReadStructure<ImageOptionalHeader32>();
			}
			else {
				OptionalHeader64 = reader.ReadStructure<ImageOptionalHeader64>();
			}

			ImageSectionHeaders = new ImageSectionHeader[FileHeader.NumberOfSections];

			for (int headerNo = 0; headerNo < ImageSectionHeaders.Length; ++headerNo) {
				ImageSectionHeaders[headerNo] = reader.ReadStructure<ImageSectionHeader>();
			}
		}


		/// <summary>
		/// Gets the header of the .NET assembly that called this function
		/// </summary>
		/// <returns></returns>
		public static PEFileReader CallingAssemblyHeader {
			get {
				// Get the path to the calling assembly, which is the path to the
				// DLL or EXE that we want the time of
				string filePath = Assembly.GetCallingAssembly().Location;

				// Get and return the timestamp
				return new PEFileReader(filePath);
			}
		}

		/// <summary>
		/// Gets the header of the .NET assembly that called this function
		/// </summary>
		/// <returns></returns>
		public static PEFileReader AssemblyHeader {
			get {
				// Get the path to the calling assembly, which is the path to the
				// DLL or EXE that we want the time of
				string filePath = Assembly.GetAssembly(typeof(PEFileReader)).Location;

				// Get and return the timestamp
				return new PEFileReader(filePath);
			}
		}

		#endregion Public Methods

		#region Properties

		/// <summary>
		/// Gets if the file header is 32 bit or not
		/// </summary>
		public bool Is32BitHeader => FileHeader.Characteristics.HasFlagFast(ImageFileCharacteristics.BIT32_MACHINE);

		public ImageDOSHeader DOSHeader { get; }

		/// <summary>
		/// Gets the file header
		/// </summary>
		public ImageFileHeader FileHeader { get; }

		/// <summary>
		/// Gets the optional header
		/// </summary>
		public ImageOptionalHeader32 OptionalHeader32 { get; }

		/// <summary>
		/// Gets the optional header
		/// </summary>
		public ImageOptionalHeader64 OptionalHeader64 { get; }

		/// <summary>
		/// Image Section headers. Number of sections is in the file header.
		/// </summary>
		public ImageSectionHeader[] ImageSectionHeaders { get; }

		/// <summary>
		/// Gets the timestamp from the file header
		/// </summary>
		public DateTime TimeStamp {
			get {
				// Timestamp is a date offset from 1970
				var date = DateTime.UnixEpoch;


				// Add in the number of seconds since 1970/1/1
				date = date.AddSeconds(FileHeader.TimeDateStamp);

				// Adjust to local timezone
				date += TimeZoneInfo.Local.GetUtcOffset(date);
				//date += TimeZone.CurrentTimeZone.GetUtcOffset(date);

				return date;
			}
		}

		[DllImport(Native.DBGHELP_DLL)]
		private static extern unsafe ImageNtHeaders64* ImageNtHeader(IntPtr hModule);

		public static unsafe ImageSectionInfo[] ReadPESectionInfo(IntPtr hModule)
		{
			// get the location of the module's IMAGE_NT_HEADERS structure
			ImageNtHeaders64* pNtHdr = ImageNtHeader(hModule);

			// section table immediately follows the IMAGE_NT_HEADERS
			var pSectionHdr = (IntPtr) (pNtHdr + 1);
			var arr         = new ImageSectionInfo[pNtHdr->FileHeader.NumberOfSections];

			int size = Marshal.SizeOf<ImageSectionHeader>();

			for (int scn = 0; scn < pNtHdr->FileHeader.NumberOfSections; ++scn) {
				var struc = Marshal.PtrToStructure<ImageSectionHeader>(pSectionHdr);
				arr[scn] = new ImageSectionInfo(struc, scn, hModule + (int) struc.VirtualAddress);

				pSectionHdr += size;
			}

			return arr;
		}

		#endregion Properties
	}
}
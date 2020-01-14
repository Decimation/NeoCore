using System;
using System.Runtime.InteropServices;
using NeoCore.Win32.Structures;

// ReSharper disable ReturnTypeCanBeEnumerable.Global

// ReSharper disable InconsistentNaming

namespace NeoCore.Win32
{
	internal static unsafe partial class Native
	{
		/// <summary>
		/// Image/PE functions from <see cref="DBGHELP_DLL"/>
		/// </summary>
		internal static class PE
		{
			[DllImport(DBGHELP_DLL)]
			private static extern ImageNtHeaders64* ImageNtHeader(IntPtr hModule);

			internal static ImageSectionInfo[] ReadPESectionInfo(IntPtr hModule)
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
		}
	}
}
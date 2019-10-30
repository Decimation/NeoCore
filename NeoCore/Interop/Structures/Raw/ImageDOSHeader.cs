using NeoCore.Interop.Attributes;

// ReSharper disable InconsistentNaming

namespace NeoCore.Interop.Structures.Raw
{
	[NativeStructure]
	public unsafe struct ImageDOSHeader
	{
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
}
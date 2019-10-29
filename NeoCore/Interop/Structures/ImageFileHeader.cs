using System.Runtime.InteropServices;
using NeoCore.Interop.Attributes;

namespace NeoCore.Interop.Structures
{
	[NativeStructure]
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct ImageFileHeader
	{
		public ushort Machine { get; }

		public ushort NumberOfSections { get; }

		public uint TimeDateStamp { get; }

		public uint PointerToSymbolTable { get; }

		public uint NumberOfSymbols { get; }

		public ushort SizeOfOptionalHeader { get; }

		public ushort Characteristics { get; }
	}
}
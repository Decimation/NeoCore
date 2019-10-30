using System.Runtime.InteropServices;
using NeoCore.Interop.Attributes;
using NeoCore.Interop.Enums;

namespace NeoCore.Interop.Structures.Raw
{
	[NativeStructure]
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct ImageFileHeader
	{
		public MachineArchitecture Machine { get; }

		public ushort NumberOfSections { get; }

		public uint TimeDateStamp { get; }

		public uint PointerToSymbolTable { get; }

		public uint NumberOfSymbols { get; }

		public ushort SizeOfOptionalHeader { get; }

		public ImageFileCharacteristics Characteristics { get; }
	}
}
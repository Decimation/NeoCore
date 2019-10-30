using System.Runtime.InteropServices;
using NeoCore.Interop.Attributes;

namespace NeoCore.Interop.Structures.Raw
{
	[NativeStructure]
	[StructLayout(LayoutKind.Sequential)]
	public struct ImageDataDirectory
	{
		public uint VirtualAddress { get; }
		public uint Size           { get; }
	}
}
using System;
using System.Runtime.InteropServices;
using NeoCore.Interop.Attributes;
using NeoCore.Interop.Enums;

namespace NeoCore.Interop.Structures
{
	[NativeStructure]
	[StructLayout(LayoutKind.Sequential)]
	internal struct MemoryBasicInformation
	{
		internal IntPtr           BaseAddress;
		internal IntPtr           AllocationBase;
		internal MemoryProtection AllocationProtect;
		internal IntPtr           RegionSize;
		internal MemState         State;
		internal MemoryProtection Protect;
		internal MemType          Type;
	}
}
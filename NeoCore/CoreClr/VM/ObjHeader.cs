using System;
using System.Runtime.InteropServices;
using NeoCore.CoreClr.Support;
using NeoCore.Interop.Attributes;
using NeoCore.Memory;
using NeoCore.Utilities.Diagnostics;

namespace NeoCore.CoreClr.VM
{
	[NativeStructure]
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct ObjHeader : IClr
	{
		// https://github.com/dotnet/coreclr/blob/master/src/gc/env/gcenv.object.h

		private IntPtr m_value;

		// DWORD 0: Sync block on x86. Padding on x64.
		// DWORD 1: Sync block on x64.

		private int this[int index] {
			get {
				fixed (ObjHeader* p = &this) {
					return ((int*) p)[index];
				}
			}
		}

		internal int Padding {
			get {
				Guard.Assert(Mem.Is64Bit);
				return this[0];
			}
		}

		public SyncBlockFlags SyncBlock {
			get {
				int i = !Mem.Is64Bit ? 0 : 1;
				return (SyncBlockFlags) this[i];
			}
		}
	}
}
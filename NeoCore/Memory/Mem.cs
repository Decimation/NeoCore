using System;
using System.Runtime.CompilerServices;

namespace NeoCore.Memory
{
	public static class Mem
	{
		public static bool Is64Bit => IntPtr.Size == sizeof(long) && Environment.Is64BitProcess;

		public static readonly Pointer<byte> Nullptr = null;
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int FullSizeOf(int elemSize, int elemCnt)
		{
			// (void*) (((long) m_value) + byteOffset)
			// (void*) (((long) m_value) + (elemOffset * ElementSize))
			return elemCnt * elemSize;
		}
	}
}
using System.Runtime.CompilerServices;

namespace NeoCore.Utilities
{
	public static class Bits
	{
		/// <summary>
		///     Reads <paramref name="bitCount" /> from <paramref name="value" /> at offset <paramref name="bitOfs" />
		/// </summary>
		/// <param name="value"><see cref="int" /> value to read from</param>
		/// <param name="bitOfs">Beginning offset</param>
		/// <param name="bitCount">Number of bits to read</param>
		public static int ReadBits(int value, int bitOfs, int bitCount)
		{
			return ((1 << bitCount) - 1) & (value >> bitOfs);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool ReadBit(int value, int bitOfs) => (value & (1 << bitOfs)) != 0;


		private static int GetMask(int bitOfs, int bitCount) => ((1 << bitCount) - 1) << bitOfs;

		public static int ReadBitsFrom(int value, int bitOfs, int bitCount)
		{
			return (value & GetMask(bitOfs, bitCount)) >> bitOfs;
		}

		public static int WriteBitsTo(int value, int bitOfs, int bitCount, int newValue)
		{
			return (value & ~GetMask(bitOfs, bitCount)) | (newValue << bitOfs);
		}
	}
}
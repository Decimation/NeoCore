using NeoCore.CoreClr;
using NeoCore.Memory.Pointers;

namespace NeoCore.Memory
{
	public readonly struct Region
	{
		public Pointer<byte> Low  { get; }
		public Pointer<byte> High { get; }
		public long          Size { get; }

		public bool HasSize      { get; }
		public bool HasAddresses { get; }

		public Region(Pointer<byte> low, Pointer<byte> high)
		{
			Low          = low;
			High         = high;
			Size         = Assets.INVALID_VALUE;
			HasSize      = false;
			HasAddresses = true;
		}

		public Region(Pointer<byte> low, long size) : this()
		{
			Low          = low;
			High         = Mem.Nullptr;
			Size         = size;
			HasSize      = true;
			HasAddresses = false;
		}
	}
}
using NeoCore.CoreClr;
using NeoCore.CoreClr.Meta;
using NeoCore.CoreClr.VM;
using NeoCore.Interop;
using NeoCore.Interop.Structures;
using NeoCore.Memory.Pointers;

namespace NeoCore.Memory
{
	public class MemInfo
	{
		public Pointer<byte> Address        { get; }
		public Pointer<byte> HighestAddress { get; private set; }
		public Pointer<byte> LowestAddress  { get; private set; }

		public MemoryType Type { get; private set; }

		public long Size { get; private set; }
		
		public MemoryBasicInfo Info { get; private set;}

		internal MemInfo(Pointer<byte> ptr)
		{
			Address = ptr;
		}


		public static MemInfo Inspect(Pointer<byte> ptr)
		{
			var info = new MemInfo(ptr);

			var gcHeap = ClrAssets.GCHeap;

			if (Mem.IsAddressInRange(ptr, gcHeap.LowestAddress, gcHeap.HighestAddress)) {
				info.LowestAddress  = gcHeap.LowestAddress;
				info.HighestAddress = gcHeap.HighestAddress;
				info.Size           = -1;
				info.Type           = MemoryType.GC;
			}

			(Pointer<byte> stackLo, long stackSize) = Mem.StackRegion;
			if (Mem.IsAddressInRange(ptr, stackLo, stackLo + stackSize)) {
				info.LowestAddress  = stackLo;
				info.HighestAddress = stackLo + stackSize;
				info.Size           = stackSize;
				info.Type           = MemoryType.Stack;
			}

			var mbi = Native.Kernel.VirtualQuery(ptr.Address);

			info.Info = mbi;

			return info;
		}

		public override string ToString()
		{
			return string.Format("Address: {0} | Type: {1} | Size: {2} | Lo: {3} | Hi: {4}", Address, Type, Size,
			                     LowestAddress, HighestAddress);
		}
	}
}
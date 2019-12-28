using System.Text;
using NeoCore.CoreClr;
using NeoCore.CoreClr.Meta;
using NeoCore.CoreClr.VM;
using NeoCore.Interop;
using NeoCore.Interop.Structures;
using NeoCore.Memory.Pointers;
using NeoCore.Support;

namespace NeoCore.Memory
{
	public sealed class MemInfo
	{
		public Pointer<byte> Address { get; }

		public Region Region { get; private set; }

		public MemoryType Type { get; private set; }
		
		public MemoryBasicInfo Info { get; private set; }

		private MemInfo(Pointer<byte> ptr)
		{
			Address = ptr;
		}


		public static MemInfo Inspect(Pointer<byte> ptr)
		{
			var info = new MemInfo(ptr);

			var gcHeap = Runtime.GC;

			var gcRegion = gcHeap.GCRegion;
			if (Mem.IsAddressInRange(ptr, gcRegion)) {
				info.Region = gcRegion;
				info.Type   = MemoryType.GC;
			}

			var stackRegion = Mem.StackRegion;
			if (Mem.IsAddressInRange(ptr, stackRegion)) {
				info.Region = stackRegion;
				info.Type   = MemoryType.Stack;
			}

			var mbi = Native.Kernel.VirtualQuery(ptr.Address);

			info.Info = mbi;

			return info;
		}

		public override string ToString()
		{
			var sb = new StringBuilder();
			sb.AppendFormat("Address: {0}\n", Address);
			sb.AppendFormat("Type: {0}", Type);
			return sb.ToString();
		}
	}
}
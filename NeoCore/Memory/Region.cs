using System.Diagnostics;
using System.Text;
using NeoCore.CoreClr;
using NeoCore.Interop.Structures;
using NeoCore.Memory.Pointers;

namespace NeoCore.Memory
{
	public readonly struct Region
	{
		public Region(Pointer<byte> low, Pointer<byte> high)
		{
			Low  = low;
			High = high;
			Size = (high - low).ToInt64() + 1;
		}

		public Region(Pointer<byte> low, long size) : this()
		{
			Low  = low;
			High = low + (size - 1);
			Size = size;
		}

		public Pointer<byte> Low  { get; }
		public Pointer<byte> High { get; }
		public long          Size { get; }

		public static Region FromProcessModule(ProcessModule module) =>
			new Region(module.BaseAddress, module.ModuleMemorySize);

		public static Region FromPage(MemoryBasicInfo info) =>
			new Region(info.BaseAddress, info.RegionSize.ToInt64());

		public static implicit operator Region(ProcessModule module) => FromProcessModule(module);

		public static implicit operator Region(MemoryBasicInfo info) => FromPage(info);


		public override string ToString()
		{
			var sb = new StringBuilder();
			sb.AppendFormat("Low: {0}\n", Low);
			sb.AppendFormat("High: {0}\n", High);
			sb.AppendFormat("Size: {0}", Size);
			return sb.ToString();
		}
	}
}
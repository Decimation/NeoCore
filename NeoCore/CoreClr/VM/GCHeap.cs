// ReSharper disable InconsistentNaming

using Memkit;
using Memkit.Pointers;
using Memkit.Utilities;
using NeoCore.Import;
using NeoCore.Import.Attributes;
using NeoCore.Model;

namespace NeoCore.CoreClr.VM
{
	[ImportNamespace(WKS_NAMESPACE)]
	public unsafe struct GCHeap : IClrStructure
	{
		private const string WKS_NAMESPACE = "WKS";

		[ImportMapField]
		private static readonly ImportMap Imports = new ImportMap();

		internal int GCCount {
			[ImportCall("GetGcCount", ImportCallOptions.Map)]
			get {
				fixed (GCHeap* value = &this) {
					return Imports.Call<int, ulong>(nameof(GCCount), (ulong) value);
				}
			}
		}

		public ClrStructureType Type => ClrStructureType.Memory;

		public string NativeName => nameof(GCHeap);

		internal bool IsHeapPointer<T>(T value, bool smallHeapOnly = false)
		{
			return Mem.TryGetAddressOfHeap(value, out Pointer<byte> ptr) &&
			       IsHeapPointer(ptr.ToPointer(), smallHeapOnly);
		}

		[ImportCall(ImportCallOptions.Map)]
		internal bool IsHeapPointer(Pointer<byte> p, bool smallHeapOnly = false)
		{
			fixed (GCHeap* value = &this) {
				return Imports.Call<bool, ulong, ulong, bool>(nameof(IsHeapPointer), (ulong) value,
				                                              p.ToUInt64(), smallHeapOnly);
			}
		}
	}
}
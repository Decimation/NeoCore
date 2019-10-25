// ReSharper disable InconsistentNaming

using NeoCore.Assets;
using NeoCore.CoreClr.Meta.Base;
using NeoCore.CoreClr.Support;
using NeoCore.Import;
using NeoCore.Import.Attributes;
using NeoCore.Interop;
using NeoCore.Memory;
using NeoCore.Memory.Pointers;

namespace NeoCore.CoreClr
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
					return Functions.Native.Call<int>((void*) Imports[nameof(GCCount)], value);
				}
			}
		}

		internal bool IsHeapPointer<T>(T value, bool smallHeapOnly = false)
		{
			return Unsafe.TryGetAddressOfHeap(value, out Pointer<byte> ptr) &&
			       IsHeapPointer(ptr.ToPointer(), smallHeapOnly);
		}

		[ImportCall(ImportCallOptions.Map)]
		internal bool IsHeapPointer(Pointer<byte> p, bool smallHeapOnly = false)
		{
			fixed (GCHeap* value = &this) {
				return Functions.Native.Call<bool, bool>((void*) Imports[nameof(IsHeapPointer)],
				                                         value, p.ToPointer(), smallHeapOnly);
			}
		}
	}
}
using NeoCore.Assets;
using NeoCore.Import;
using NeoCore.Import.Attributes;
using NeoCore.Interop;
using NeoCore.Memory;
using NeoCore.Memory.Pointers;

// ReSharper disable InconsistentNaming

namespace NeoCore.CoreClr
{
	[ImportNamespace(WKS_NAMESPACE)]
	public static class GCHeap
	{
		private const string WKS_NAMESPACE = "WKS";

		static GCHeap()
		{
			ImportManager.Value.Load(typeof(GCHeap), Resources.Clr.Imports);
		}

		[ImportField(IdentifierOptions.FullyQualified, ImportFieldOptions.Fast)]
		private static readonly Pointer<byte> g_pGCHeap;

		[ImportMapField]
		private static readonly ImportMap Imports = new ImportMap();

		public static int GCCount {
			[ImportCall("GetGcCount", ImportCallOptions.Map)]
			get {
				unsafe {
					return Functions.Native.Call<int>((void*) Imports[nameof(GCCount)], g_pGCHeap.ToPointer());
				}
			}
		}
		
		public static bool IsHeapPointer<T>(T value, bool smallHeapOnly = false)
		{
			unsafe {
				return Unsafe.TryGetAddressOfHeap(value, out Pointer<byte> ptr) &&
				       IsHeapPointer(ptr.ToPointer(), smallHeapOnly);
			}
		}

		[ImportCall(ImportCallOptions.Map)]
		public static bool IsHeapPointer(Pointer<byte> p, bool smallHeapOnly = false)
		{
			unsafe {
				return Functions.Native.Call<bool, bool>((void*) Imports[nameof(IsHeapPointer)],
				                                         g_pGCHeap.ToPointer(), p.ToPointer(), smallHeapOnly);
			}
		}
	}
}
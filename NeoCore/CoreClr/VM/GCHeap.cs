// ReSharper disable InconsistentNaming

using System;
using System.Runtime.CompilerServices;
using Memkit;
using Memkit.Pointers;
using Memkit.Utilities;
using NeoCore.CoreClr.Meta;
using NeoCore.Import;
using NeoCore.Import.Attributes;
using NeoCore.Model;
using NeoCore.Utilities;

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

		
		// todo: fix
		
		/*[ImportCall(IdentifierOptions.FullyQualified, ImportCallOptions.Map)]
		internal void* AllocateObject(Pointer<MethodTable> mt, bool fHandleCom = false)
		{
			fixed (GCHeap* value = &this) {
				return Imports.CallReturnPointer(nameof(AllocateObject),(long) value, mt.ToInt64(), Convert.ToInt64(fHandleCom));
			}
			
		}
		
		
		
		internal object AllocateObject(MetaType mt, bool fHandleCom = false)
		{
			void* p = AllocateObject(mt.Value.ToPointer(), fHandleCom);
			return Unsafe.Read<object>(&p);
		}
		
		internal T AllocateObject<T>(bool fHandleCom = false)
		{
			return (T) AllocateObject(typeof(T), fHandleCom);
		}*/

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
		
		// https://github.com/Decimation/RazorSharp/blob/master/RazorSharp/CoreClr/GCHeap.cs
	}
}
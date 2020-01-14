using System;
using System.Reflection;
using System.Text;
using Memkit;
using Memkit.Model;
using Memkit.Pointers;
using NeoCore.CoreClr.Meta.Base;
using NeoCore.CoreClr.VM;
using NeoCore.Support;

// ReSharper disable InconsistentNaming

namespace NeoCore.CoreClr.Meta
{
	/// <summary>
	/// Represents a CLR GC heap.
	///     <list type="bullet">
	///         <item><description>CLR structure: <see cref="GCHeap"/></description></item>
	///     </list>
	/// </summary>
	public sealed unsafe class MetaHeap : BasicClrStructure<GCHeap>
	{
		/// <summary>
		/// Represents the global CLR GC heap.
		/// </summary>
		public static readonly MetaHeap GC = ReadGC();

		internal MetaHeap(Pointer<GCHeap> ptr, Pointer<byte> lo, Pointer<byte> hi) : base(ptr)
		{
			GCRegion = new Region(lo,hi);
		}

		protected override Type[] AdditionalSources => null;

		public Region GCRegion { get; }

		public int GCCount => Value.Reference.GCCount;

		public bool IsHeapPointer<T>(T v, bool smallHeapOnly = false)
		{
			return Value.Reference.IsHeapPointer(v, smallHeapOnly);
		}

		public bool IsHeapPointer(Pointer<byte> p, bool smallHeapOnly = false)
		{
			return Value.Reference.IsHeapPointer(p, smallHeapOnly);
		}

		public override string ToString()
		{
			var sb = new StringBuilder();
			sb.AppendFormat("Heap: {0}\n", Value);
			sb.AppendFormat("Lowest address: {0}\n", GCRegion.Low);
			sb.AppendFormat("Highest address: {0}", GCRegion.High);
			return sb.ToString();
		}

		private static MetaHeap ReadGC()
		{
			var clr = Resources.Clr.Imports;

			const string GLOBAL_GCHEAP_PTR = "g_pGCHeap";
			const string GLOBAL_GCHEAP_LO  = "g_lowest_address";
			const string GLOBAL_GCHEAP_HI  = "g_highest_address";

			Pointer<byte> gc = clr.GetAddress(GLOBAL_GCHEAP_PTR);
			Pointer<byte> lo = clr.GetAddress(GLOBAL_GCHEAP_LO).ReadPointer();
			Pointer<byte> hi = clr.GetAddress(GLOBAL_GCHEAP_HI).ReadPointer();

			return new MetaHeap(gc, lo, hi);
		}
	}
}
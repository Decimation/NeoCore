using System;
using System.Reflection;
using System.Text;
using NeoCore.CoreClr.Meta.Base;
using NeoCore.CoreClr.VM;
using NeoCore.Memory.Pointers;

// ReSharper disable InconsistentNaming

namespace NeoCore.CoreClr.Meta
{
	/// <summary>
	/// Represents a CLR GC heap.
	///     <list type="bullet">
	///         <item><description>CLR structure: <see cref="GCHeap"/></description></item>
	///     </list>
	/// </summary>
	public sealed unsafe class MetaHeap : AnonymousClrStructure<GCHeap>
	{
		internal MetaHeap(Pointer<GCHeap> ptr, Pointer<byte> lo, Pointer<byte> hi) : base(ptr)
		{
			LowestAddress = lo;
			HighestAddress = hi;
		}

		protected override Type[] AdditionalSources => null;

		public Pointer<byte> LowestAddress { get; }
		
		public Pointer<byte> HighestAddress { get; }
		
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
			sb.AppendFormat("Lowest address: {0}\n", LowestAddress);
			sb.AppendFormat("Highest address: {0}", HighestAddress);
			return sb.ToString();
		}
	}
}
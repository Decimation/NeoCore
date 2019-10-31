using System;
using System.Reflection;
using NeoCore.CoreClr.Components;
using NeoCore.CoreClr.Meta.Base;
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
	public sealed class MetaHeap : AnonymousClrStructure<GCHeap>
	{
		internal MetaHeap(Pointer<GCHeap> ptr) : base(ptr) { }

		protected override Type[] AdditionalSources => null;

		public int GCCount => Value.Reference.GCCount;

		public bool IsHeapPointer<T>(T v, bool smallHeapOnly = false)
		{
			return Value.Reference.IsHeapPointer(v, smallHeapOnly);
		}

		public bool IsHeapPointer(Pointer<byte> p, bool smallHeapOnly = false)
		{
			return Value.Reference.IsHeapPointer(p, smallHeapOnly);
		}
	}
}
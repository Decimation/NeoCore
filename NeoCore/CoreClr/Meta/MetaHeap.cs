using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Memkit;
using Memkit.Model;
using Memkit.Pointers;
using Memkit.Utilities;
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
	/// <seealso cref="Activator"/>
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

		private const bool HANDLE_COM = false;
		
		/*public object AllocObject(MetaType mt)
		{
			var val= Value.Reference.AllocateObject(mt, HANDLE_COM);

			return val;
		}

		public T AllocObject<T>() => Value.Reference.AllocateObject<T>(HANDLE_COM);

		public Pointer<byte> AllocObject(Pointer<MethodTable> mt) => Value.Reference.AllocateObject(mt,HANDLE_COM);*/
		
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

			// .data	0000000180517000	000000018052F000	R	W	.	.	L	para	0004	public	DATA	64	0000	0000	0004	FFFFFFFFFFFFFFFF	FFFFFFFFFFFFFFFF

			// 000000018051C120
			var segments=PEFileReader.ReadPESectionInfo(Resources.Clr.Module.BaseAddress);
			
			var text=segments.Single(s => s.Name.Contains(".data"));
			
			var h = 0x5120;

			Pointer<byte> gc = text.Address + h;
			//Pointer<byte> lo = clr.GetAddress(GLOBAL_GCHEAP_LO).ReadPointer();
			//Pointer<byte> hi = clr.GetAddress(GLOBAL_GCHEAP_HI).ReadPointer();

			return new MetaHeap(gc.ReadPointer(), null, null);
		}
		
		
		// https://gist.github.com/afish/de9efe886164e9cdea74
		// https://gist.github.com/afish/33046b6c90833c922d6d
	}
}
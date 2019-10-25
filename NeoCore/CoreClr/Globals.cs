using NeoCore.Assets;
using NeoCore.CoreClr.Meta;
using NeoCore.Import;
using NeoCore.Import.Attributes;
using NeoCore.Interop;
using NeoCore.Memory;
using NeoCore.Memory.Pointers;

// ReSharper disable InconsistentNaming

namespace NeoCore.CoreClr
{
	/// <summary>
	/// Contains global CLR variables.
	/// </summary>
	[ImportNamespace]
	public static class Globals
	{
		static Globals()
		{
			ImportManager.Value.Load(typeof(Globals), Resources.Clr.Imports);
		}
		
		[ImportGlobalField(ImportFieldOptions.Fast)]
		private static readonly Pointer<GCHeap> g_pGCHeap;
		
		/// <summary>
		/// Represents the global CLR GC heap.
		/// </summary>
		public static readonly MetaHeap GCHeap = new MetaHeap(g_pGCHeap);
	}
}
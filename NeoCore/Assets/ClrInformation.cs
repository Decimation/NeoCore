using System.Runtime.CompilerServices;
using NeoCore.CoreClr.Meta;
using NeoCore.CoreClr.VM;
using NeoCore.Import;
using NeoCore.Import.Attributes;
using NeoCore.Memory;
using NeoCore.Memory.Pointers;

// ReSharper disable InconsistentNaming
#pragma warning disable 649

namespace NeoCore.Assets
{
	/// <summary>
	/// Contains global CLR variables, offsets, sizes, and other constants.
	/// </summary>
	[ImportNamespace]
	public static class ClrInformation
	{
		static ClrInformation()
		{
			ImportManager.Value.Load(typeof(ClrInformation), Resources.Clr.Imports);
			GCHeap = new MetaHeap(g_pGCHeap);
		}

		[ImportGlobalField(ImportFieldOptions.Fast)]
		private static readonly Pointer<GCHeap> g_pGCHeap;

		/// <summary>
		/// Represents the global CLR GC heap.
		/// </summary>
		public static readonly MetaHeap GCHeap;

		#region Sizes-

		// https://github.com/dotnet/coreclr/blob/master/src/vm/object.h

		/// <summary>
		/// <list type="bullet">
		///         <item>
		///             <description>+ 2: <see cref="ObjHeader.Padding"/> (x64)</description>
		///         </item>
		///         <item>
		///             <description>+ 2: <see cref="ObjHeader.SyncBlock"/></description>
		///         </item>
		///     </list>
		/// </summary>
		public static readonly unsafe int ObjHeaderSize = sizeof(ObjHeader);

		/// <summary>
		///     Size of <see cref="TypeHandle" /> and <see cref="ObjHeader" />
		///     <list type="bullet">
		///         <item>
		///             <description>+ <see cref="ClrInformation.ObjHeaderSize" />: <see cref="ObjHeader" /></description>
		///         </item>
		///         <item>
		///             <description>+ <see cref="Mem.Size" />: <see cref="TypeHandle"/></description>
		///         </item>
		///     </list>
		/// </summary>
		public static readonly unsafe int ObjectBaseSize = ClrInformation.ObjHeaderSize + sizeof(TypeHandle);


		/// <summary>
		///     Size of the length field and padding (x64)
		/// </summary>
		public static readonly int ArrayOverhead = Mem.Size;

		/// <summary>
		///     Size of the length field and first character
		///     <list type="bullet">
		///         <item>
		///             <description>+ 2: First character</description>
		///         </item>
		///         <item>
		///             <description>+ 4: String length</description>
		///         </item>
		///     </list>
		/// </summary>
		public static readonly int StringOverhead = sizeof(char) + sizeof(int);

		/// <summary>
		///     <para>Minimum GC object heap size</para>
		/// </summary>
		public static readonly int MinObjectSize = (Mem.Size * 2) + ClrInformation.ObjHeaderSize;

		#endregion

		#region Offset

		/// <summary>
		///     Heap offset to the first field.
		///     <list type="bullet">
		///         <item>
		///             <description>+ <see cref="Mem.Size" /> for <see cref="TypeHandle" /></description>
		///         </item>
		///     </list>
		/// </summary>
		public static readonly int OffsetToData = Mem.Size;

		/// <summary>
		///     Heap offset to the first array element.
		///     <list type="bullet">
		///         <item>
		///             <description>+ <see cref="Mem.Size" /> for <see cref="TypeHandle" /></description>
		///         </item>
		///         <item>
		///             <description>+ 4 for length (<see cref="uint" />) </description>
		///         </item>
		///         <item>
		///             <description>+ 4 for padding (<see cref="uint" />) (x64 only)</description>
		///         </item>
		///     </list>
		/// </summary>
		public static readonly int OffsetToArrayData = ClrInformation.OffsetToData + ArrayOverhead;

		/// <summary>
		///     Heap offset to the first string character.
		/// On 64 bit platforms, this should be 12 (8 + 4) and on 32 bit 8 (4 + 4).
		/// (<see cref="Mem.Size"/> + <see cref="int"/>)
		/// </summary>
		public static readonly int OffsetToStringData = RuntimeHelpers.OffsetToStringData;

		#endregion

		/// <summary>
		/// Common value representing an invalid value or a failure
		/// </summary>
		internal const int INVALID_VALUE = -1;

		internal const int BITS_PER_DWORD = 32;
	}
}

#pragma warning restore 649
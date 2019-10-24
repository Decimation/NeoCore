using System.Runtime.CompilerServices;
using NeoCore.CoreClr;
using NeoCore.CoreClr.VM;
using NeoCore.Memory;

namespace NeoCore.Assets
{
	public static partial class Constants
	{
		/// <summary>
		///     Common runtime offsets.
		/// </summary>
		public static unsafe class Offsets
		{
			/// <summary>
			///     Heap offset to the first field.
			///     <list type="bullet">
			///         <item>
			///             <description>+ <see cref="Mem.PointerSize" /> for <see cref="TypeHandle" /></description>
			///         </item>
			///     </list>
			/// </summary>
			public static readonly int OffsetToData = Mem.PointerSize;

			/// <summary>
			///     Heap offset to the first array element.
			///     <list type="bullet">
			///         <item>
			///             <description>+ <see cref="Mem.PointerSize" /> for <see cref="TypeHandle" /></description>
			///         </item>
			///         <item>
			///             <description>+ 4 for length (<see cref="uint" />) </description>
			///         </item>
			///         <item>
			///             <description>+ 4 for padding (<see cref="uint" />) (x64 only)</description>
			///         </item>
			///     </list>
			/// </summary>
			public static readonly int OffsetToArrayData = OffsetToData + Sizes.ArrayOverhead;

			/// <summary>
			///     Heap offset to the first string character.
			/// On 64 bit platforms, this should be 12 (8 + 4) and on 32 bit 8 (4 + 4).
			/// (<see cref="Mem.PointerSize"/> + <see cref="int"/>)
			/// </summary>
			public static readonly int OffsetToStringData = RuntimeHelpers.OffsetToStringData;
		}
	}
}
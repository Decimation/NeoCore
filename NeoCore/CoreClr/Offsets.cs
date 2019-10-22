using System;
using System.Runtime.CompilerServices;
using NeoCore.Memory;

namespace NeoCore.CoreClr
{
	/// <summary>
	///     Common runtime offsets.
	/// </summary>
	public static class Offsets
	{
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
		///     Size of the length field and padding (x64)
		/// </summary>
		public static readonly int ArrayOverhead = Mem.PointerSize;

		/// <summary>
		///     Size of <see cref="TypeHandle" /> and <see cref="ObjHeader" />
		///     <list type="bullet">
		///         <item>
		///             <description>+ <see cref="Mem.PointerSize" />: <see cref="ObjHeader" /></description>
		///         </item>
		///         <item>
		///             <description>+ <see cref="Mem.PointerSize" />: <see cref="MethodDesc" /> pointer</description>
		///         </item>
		///     </list>
		/// </summary>
		public static readonly int ObjectOverhead = Mem.PointerSize * 2;

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
		///             <description>+ 4 for length (<see cref="UInt32" />) </description>
		///         </item>
		///         <item>
		///             <description>+ 4 for padding (<see cref="UInt32" />) (x64 only)</description>
		///         </item>
		///     </list>
		/// </summary>
		public static readonly int OffsetToArrayData = OffsetToData + ArrayOverhead;

		/// <summary>
		///     Heap offset to the first string character.
		/// On 64 bit platforms, this should be 12 (8 + 4) and on 32 bit 8 (4 + 4).
		/// (<see cref="Mem.PointerSize"/> + <see cref="int"/>)
		/// </summary>
		public static readonly int OffsetToStringData = RuntimeHelpers.OffsetToStringData;

		/// <summary>
		///     <para>Minimum GC object heap size</para>
		///     <para>Sources:</para>
		///     <list type="bullet">
		///         <item>
		///             <description>/src/vm/object.h: 119</description>
		///         </item>
		///     </list>
		/// </summary>
		public static readonly int MinObjectSize = ObjectOverhead + Mem.PointerSize;
	}
}
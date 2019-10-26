using NeoCore.CoreClr.VM;
using NeoCore.Memory;

namespace NeoCore.Assets
{
	public static partial class Constants
	{
		public static unsafe class Sizes
		{
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
			public static readonly int ObjHeaderSize = sizeof(ObjHeader);

			/// <summary>
			///     Size of <see cref="TypeHandle" /> and <see cref="ObjHeader" />
			///     <list type="bullet">
			///         <item>
			///             <description>+ <see cref="ObjHeaderSize" />: <see cref="ObjHeader" /></description>
			///         </item>
			///         <item>
			///             <description>+ <see cref="Mem.Size" />: <see cref="TypeHandle"/></description>
			///         </item>
			///     </list>
			/// </summary>
			public static readonly int ObjectBaseSize = ObjHeaderSize + sizeof(TypeHandle);

			/// <summary>
			///     <para>Minimum GC object heap size</para>
			/// </summary>
			public static readonly int MinObjectSize = (Mem.Size * 2) + ObjHeaderSize;


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
		}
	}
}
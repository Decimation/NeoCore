using System.Drawing;
using System.Runtime.CompilerServices;
using Memkit;
using Memkit.Utilities;
using NeoCore.CoreClr.Meta;
using NeoCore.CoreClr.VM;
using NeoCore.Import.Attributes;
using NeoCore.Memory;
using NeoCore.Support;

// ReSharper disable InconsistentNaming
#pragma warning disable 649

namespace NeoCore.CoreClr
{
	/// <summary>
	/// Contains offsets, sizes, and other constants.
	/// </summary>
	public static class ClrAssets
	{
		#region Sizes

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
		///             <description>+ <see cref="ClrAssets.ObjHeaderSize" />: <see cref="ObjHeader" /></description>
		///         </item>
		///         <item>
		///             <description>+ <see cref="Size" />: <see cref="TypeHandle"/></description>
		///         </item>
		///     </list>
		/// </summary>
		public static readonly unsafe int ObjectBaseSize = ClrAssets.ObjHeaderSize + sizeof(TypeHandle);

		/// <summary>
		///     <para>Minimum GC object heap size</para>
		/// </summary>
		public static readonly int MinObjectSize = (Mem.Size * 2) + ClrAssets.ObjHeaderSize;

		#endregion
	}
}

#pragma warning restore 649
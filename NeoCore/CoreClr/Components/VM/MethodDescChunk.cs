using System.Runtime.InteropServices;
using NeoCore.Assets;
using NeoCore.CoreClr.Meta.Base;
using NeoCore.Import.Attributes;
using NeoCore.Interop.Attributes;
using NeoCore.Memory;
using NeoCore.Memory.Pointers;

namespace NeoCore.CoreClr.Components.VM
{
	[ImportNamespace]
	[NativeStructure]
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct MethodDescChunk : IClrStructure
	{
		/// <summary>
		/// <see cref="RelativeFixupPointer{T}"/>
		/// </summary>
		private RelativeFixupPointer<MethodTable> MethodTableStub { get; }

		/// <summary>
		/// <see cref="RelativePointer{T}"/> to <see cref="MethodDescChunk"/>
		/// </summary>
		private RelativePointer<byte> Next { get; }

		/// <summary>
		/// The size of this chunk minus 1 (in multiples of MethodDesc::ALIGNMENT)
		/// </summary>
		internal byte Size { get; }

		/// <summary>
		/// The number of MethodDescs in this chunk minus 1
		/// </summary>
		internal byte Count { get; }

		internal ChunkFlags FlagsAndTokenRange { get; }

		// Followed by array of method descs...

		internal Pointer<MethodTable> MethodTable {
			get {
				// for MDC: m_methodTable.GetValue(PTR_HOST_MEMBER_TADDR(MethodDescChunk, this, m_methodTable));

				const int MT_FIELD_OFS = 0;
				return Structures.FieldOffset(MethodTableStub.NativeValue, MT_FIELD_OFS);
			}
		}

		public ClrStructureType Type => ClrStructureType.Metadata;
	}
}
using System.Runtime.InteropServices;
using NeoCore.CoreClr.Support;
using NeoCore.Interop.Attributes;
using NeoCore.Memory;

namespace NeoCore.CoreClr.Metadata
{
	[NativeStructure]
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct MethodDescChunk
	{
		// RelativeFixupPointer
		//internal MethodTable* MethodTableRaw { get; }
		internal RelativeFixupPointer<SimplePointer<byte>> MethodTableRaw { get; }
		
		// RelativePointer
		//internal MethodDescChunk* Next { get; }
		internal RelativePointer<SimplePointer<byte>> Next { get; }
		
		/// <summary>
		/// The size of this chunk minus 1 (in multiples of MethodDesc::ALIGNMENT)
		/// </summary>
		internal byte Size { get; }
		
		/// <summary>
		/// The number of MethodDescs in this chunk minus 1
		/// </summary>
		internal byte Count { get; }

		internal ushort FlagsAndTokenRange { get; }
		
		// Followed by array of method descs...
		
		internal Pointer<MethodTable>  MethodTable
		{
			get {
				// for MDC: m_methodTable.GetValue(PTR_HOST_MEMBER_TADDR(MethodDescChunk, this, m_methodTable));

				//const int MT_FIELD_OFS = 0;
				//return (MethodTable*) (MT_FIELD_OFS + ((long) MethodTableRaw));

				fixed (MethodDescChunk* value = &this) {
					return MethodTableRaw.GetValue((QInt) ClrAccess.HostMemberAddress(value, nameof(MethodTableRaw), true).ToInt64()).Cast<MethodTable>();
				}
				
			}
		}
	}
}
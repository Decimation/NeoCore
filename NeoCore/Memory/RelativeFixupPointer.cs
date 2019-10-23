using System;
using NeoCore.CoreClr.Support;

namespace NeoCore.Memory
{
	// todo: WIP
	public unsafe struct RelativeFixupPointer<T> where T : unmanaged
	{
		private ulong m_delta;

		// Returns value of the encoded pointer. Assumes that the pointer is not NULL.
		public Pointer<T> GetValue(ulong value)
		{
			const ulong FIXUP_POINTER_INDIRECTION = 1;

//			PRECONDITION(!IsNull());
//			PRECONDITION(!IsTagged(base));
//			TADDR addr = base + m_delta;
//			if ((addr & FIXUP_POINTER_INDIRECTION) != 0)
//				addr = *PTR_TADDR(addr - FIXUP_POINTER_INDIRECTION);
//			return dac_cast<PTR_TYPE>(addr);

			ulong addr = value + m_delta;

			if ((addr & FIXUP_POINTER_INDIRECTION) != 0) {
				//addr = *PTR_TADDR(addr - FIXUP_POINTER_INDIRECTION);
				// ???
				addr = *((ulong*) (addr - FIXUP_POINTER_INDIRECTION));
			}

			return (Pointer<T>) addr;
		}

		public override string ToString()
		{
			return String.Format("{0:X} d", m_delta);
		}
	}
}
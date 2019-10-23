using NeoCore.CoreClr.Support;

namespace NeoCore.Memory
{
	public unsafe struct RelativeFixupPointer<T> where T : unmanaged
	{
		private QInt m_delta;
		
		// Returns value of the encoded pointer. Assumes that the pointer is not NULL.
		public T GetValue(QInt value)
		{
			const ulong FIXUP_POINTER_INDIRECTION = 1;
			
//			PRECONDITION(!IsNull());
//			PRECONDITION(!IsTagged(base));
//			TADDR addr = base + m_delta;
//			if ((addr & FIXUP_POINTER_INDIRECTION) != 0)
//				addr = *PTR_TADDR(addr - FIXUP_POINTER_INDIRECTION);
//			return dac_cast<PTR_TYPE>(addr);

			QInt addr = value + m_delta;
			
			if ((addr & FIXUP_POINTER_INDIRECTION) != 0) {
				//addr = *PTR_TADDR(addr - FIXUP_POINTER_INDIRECTION);
				
				// ???
				addr = *((QInt*) (addr - FIXUP_POINTER_INDIRECTION).Value);
			}

			return ClrAccess.Cast<QInt, T>(addr);
		}
	}
}
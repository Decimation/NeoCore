using NeoCore.CoreClr.Support;

namespace NeoCore.Memory
{
	public unsafe struct RelativePointer<T> where T : unmanaged
	{
		private QInt m_delta;

		// Returns value of the encoded pointer. Assumes that the pointer is not NULL.
		public T GetValue(QInt value)
		{
			// return dac_cast<PTR_TYPE>(base + m_delta);

			return ClrAccess.Cast<QInt, T>(value + m_delta);
		}
	}
}
using NeoCore.CoreClr.Support;
using NeoCore.Interop.Attributes;

namespace NeoCore.Memory
{
	// todo: WIP
	
	[NativeStructure]
	public unsafe struct RelativePointer<T> where T : unmanaged
	{
		private QInt m_delta;

		// Returns value of the encoded pointer. Assumes that the pointer is not NULL.
		public T GetValue(QInt value)
		{
			// return dac_cast<PTR_TYPE>(base + m_delta);

			return default;
		}
	}
}
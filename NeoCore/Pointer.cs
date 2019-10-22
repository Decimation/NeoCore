using System;

namespace NeoCore
{
	public unsafe struct Pointer<T>
	{
		private void* m_value;

		public Pointer(void* value)
		{
			m_value = value;
		}
		
		public static implicit operator Pointer<T>(void* value) => new Pointer<T>(value);

		public override string ToString()
		{
			return String.Format("{0:X}", new IntPtr(m_value).ToInt64());
		}
	}
}
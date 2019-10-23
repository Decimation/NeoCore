namespace NeoCore.Memory
{
	public unsafe struct SimplePointer<T> where T : unmanaged
	{
		private T* m_value;

		public SimplePointer(T* value)
		{
			m_value = value;
		}
		
		public SimplePointer(void* value)
		{
			m_value = (T*) value;
		}
		
		public SimplePointer<TNew> Cast<TNew>() where TNew : unmanaged
		{
			return new SimplePointer<TNew>((byte*) m_value);
		}
		
		public static implicit operator SimplePointer<T>(void* ptr) => new SimplePointer<T>(ptr);
		
		public static implicit operator Pointer<T>(SimplePointer<T> ptr) => ptr.m_value;
		
	}
}
namespace NeoCore.Memory.Pointers
{
	public unsafe interface IRelativePointer<T> : IPointer<T> where T : unmanaged
	{
		// => (T*) Value;
		T* NativeValue { get; }
		
		ulong Value { get; }
		
		Pointer<T> GetValue(ulong value);
	}
}
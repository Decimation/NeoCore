namespace NeoCore.Memory.Pointers
{
	public interface IRelativePointer<T> : IPointer<T>
	{
		ulong Value { get; }
		
		Pointer<T> GetValue(ulong value);
	}
}
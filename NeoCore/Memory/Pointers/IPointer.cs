namespace NeoCore.Memory.Pointers
{
	public interface IPointer<T>
	{
		T Read();
	}
}
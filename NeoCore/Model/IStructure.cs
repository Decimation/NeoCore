using NeoCore.Memory;

namespace NeoCore.Model
{
	public interface IStructure
	{
		string Name { get; }

		int Offset { get; }

		int Size { get; }

		Pointer<byte> GetAddress<T>(ref T value);

		object GetValue(object value);
	}
}
using Memkit;
using Memkit.Pointers;
using NeoCore.Model;

// ReSharper disable InconsistentNaming

namespace NeoCore.CoreClr.VM.Jit
{
	public interface ICorMethodStructure : IClrStructure
	{
		int CodeSize { get; }

		Pointer<byte> Code { get; }

		int MaxStackSize { get; }

		int LocalVarSigToken { get; }

		byte[] CodeIL { get; }
	}
}
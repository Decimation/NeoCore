using NeoCore.Assets;
using NeoCore.Memory.Pointers;

// ReSharper disable InconsistentNaming

namespace NeoCore.CoreClr.Components.VM.Jit
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
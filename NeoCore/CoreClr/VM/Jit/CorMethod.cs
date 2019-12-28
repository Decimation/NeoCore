using System.Runtime.InteropServices;
using NeoCore.Import.Attributes;
using NeoCore.Interop.Attributes;
using NeoCore.Memory.Pointers;
using NeoCore.Model;


// ReSharper disable InconsistentNaming

namespace NeoCore.CoreClr.VM.Jit
{
	[ImportNamespace]
	[NativeStructure]
	[StructLayout(LayoutKind.Explicit)]
	public unsafe struct CorMethod : ICorMethodStructure
	{
		// COR_ILMETHOD
		// https://github.com/dotnet/coreclr/blob/master/src/inc/corhlpr.h#L597
		// https://github.com/dotnet/coreclr/blob/master/src/inc/corhdr.h

		[field: FieldOffset(0)]
		private CorMethodFat m_fat;

		[field: FieldOffset(0)]
		private CorMethodTiny m_tiny;

		private ref CorMethodFat Fat {
			get {
				fixed (CorMethod* value = &this) {
					var p = (Pointer<CorMethodFat>) (&value->m_fat);
					return ref p.Reference;
				}
			}
		}

		private ref CorMethodTiny Tiny {
			get {
				fixed (CorMethod* value = &this) {
					var p = (Pointer<CorMethodTiny>) (&value->m_tiny);
					return ref p.Reference;
				}
			}
		}

		internal bool IsFat => Fat.IsFat;

		internal bool IsTiny => Tiny.IsTiny;
		
		public int CodeSize => IsFat ? Fat.CodeSize : Tiny.CodeSize;

		public Pointer<byte> Code => IsFat ? Fat.Code : Tiny.Code;

		public byte[] CodeIL => IsFat ? Fat.CodeIL : Tiny.CodeIL;

		public int MaxStackSize => IsFat ? Fat.MaxStackSize : Tiny.MaxStackSize;

		public int LocalVarSigToken => IsFat ? Fat.LocalVarSigToken : Tiny.LocalVarSigToken;

		public ClrStructureType Type => ClrStructureType.Metadata;
		public string NativeName => null;
	}
}
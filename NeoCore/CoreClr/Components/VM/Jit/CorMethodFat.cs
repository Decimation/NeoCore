using System.Runtime.InteropServices;
using NeoCore.CoreClr.Components.Support;
using NeoCore.Import.Attributes;
using NeoCore.Interop.Attributes;
using NeoCore.Memory.Pointers;

namespace NeoCore.CoreClr.Components.VM.Jit
{
	[ImportNamespace]
	[NativeStructure]
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct CorMethodFat : IClrStructure
	{
		// IMAGE_COR_ILMETHOD_FAT
		// COR_ILMETHOD_FAT
		
		// https://github.com/dotnet/coreclr/blob/master/src/inc/corhlpr.h
		// https://github.com/dotnet/coreclr/blob/master/src/inc/corhdr.h
		
		// This strucuture is the 'fat' layout, where no compression is attempted.
		// Note that this structure can be added on at the end, thus making it extensible

//		unsigned Flags    : 12; // Flags see code:CorILMethodFlags
//		unsigned Size     :  4; // size in DWords of this structure (currently 3)
//		unsigned MaxStack : 16; // maximum number of items (I4, I, I8, obj ...), on the operand stack
		private int m_bf0;

		private int m_codeSize;
		private int m_localVarSigTok;

		internal bool IsFat {
			get {
				fixed (CorMethodFat* value = &this) {
					return (*(byte*) value & (int) CorILMethodFlags.FatFormat) ==
					       (int) CorILMethodFlags.FatFormat;
				}
			}
		}

		internal int Size {
			get {
				fixed (CorMethodFat* value = &this) {
					var p = (byte*) value;
					return *(p + 1) >> 4;
				}
			}
		}

		internal Pointer<byte> Code {
			get {
				fixed (CorMethodFat* value = &this) {
					var p = (byte*) value;
					return ((p) + 4 * Size);
				}
			}
		}

		public ClrStructureType Type => ClrStructureType.Metadata;
	}
}
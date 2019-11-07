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
	public unsafe struct CorMethodTiny : IClrStructure
	{
		// IMAGE_COR_ILMETHOD_TINY
		// COR_ILMETHOD_TINY
		/* Used when the method is tiny (< 64 bytes), and there are no local vars */
		
		// https://github.com/dotnet/coreclr/blob/master/src/inc/corhlpr.h
		// https://github.com/dotnet/coreclr/blob/master/src/inc/corhdr.h
		
		private byte m_flagsAndCodeSize;

		internal bool IsTiny {
			get {
				// return((Flags_CodeSize & (CorILMethod_FormatMask >> 1)) == CorILMethod_TinyFormat);
				return (m_flagsAndCodeSize & (((int) CorILMethodFlags.FormatMask) >> 1))
				       == (int) CorILMethodFlags.TinyFormat;
			}
		}

		internal int CodeSize {
			get {
				// return(((unsigned) Flags_CodeSize) >> (CorILMethod_FormatShift-1));
				return (m_flagsAndCodeSize) >> ((int) (CorILMethodFlags.FormatShift - 1));
			}
		}

		internal Pointer<byte> Code {
			get {
				// return(((BYTE*) this) + sizeof(struct tagCOR_ILMETHOD_TINY));

				fixed (CorMethodTiny* value = &this) {
					return (((byte*) value) + sizeof(CorMethodTiny));
				}
			}
		}

		public ClrStructureType Type => ClrStructureType.Metadata;
	}
}
using System.Runtime.InteropServices;
using NeoCore.Assets;
using NeoCore.Import.Attributes;
using NeoCore.Interop.Attributes;
using NeoCore.Memory.Pointers;

// ReSharper disable ArrangeAccessorOwnerBody
// ReSharper disable InconsistentNaming

namespace NeoCore.CoreClr.VM.Jit
{
	[ImportNamespace]
	[NativeStructure]
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct CorMethodTiny : ICorMethodStructure
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

		public int CodeSize {
			get {
				// return(((unsigned) Flags_CodeSize) >> (CorILMethod_FormatShift-1));
				return (m_flagsAndCodeSize) >> ((int) (CorILMethodFlags.FormatShift - 1));
			}
		}

		public Pointer<byte> Code {
			get {
				// return(((BYTE*) this) + sizeof(struct tagCOR_ILMETHOD_TINY));

				fixed (CorMethodTiny* value = &this) {
					return (((byte*) value) + sizeof(CorMethodTiny));
				}
			}
		}


		public int MaxStackSize {
			get {
				const int MAX_STACK_SIZE_TINY = 8;
				return MAX_STACK_SIZE_TINY;
			}
		}

		public int LocalVarSigToken {
			get {
				const int LOCAL_VAR_SIG_TOKEN_TINY = 0;
				return LOCAL_VAR_SIG_TOKEN_TINY;
			}
		}

		public byte[] CodeIL => Code.Copy(CodeSize);

		public ClrStructureType Type => ClrStructureType.Metadata;
	}
}
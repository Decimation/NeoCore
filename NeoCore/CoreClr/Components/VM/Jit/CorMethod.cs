using System.Runtime.InteropServices;
using NeoCore.CoreClr.Components.Support;
using NeoCore.Import.Attributes;
using NeoCore.Interop.Attributes;

// ReSharper disable InconsistentNaming

namespace NeoCore.CoreClr.Components.VM.Jit
{
	[ImportNamespace]
	[NativeStructure]
	[StructLayout(LayoutKind.Explicit)]
	public unsafe struct CorMethod : IClrStructure
	{
		// COR_ILMETHOD
		// https://github.com/dotnet/coreclr/blob/master/src/inc/corhlpr.h#L597
		// https://github.com/dotnet/coreclr/blob/master/src/inc/corhdr.h
		
		[field: FieldOffset(0)]
		private CorMethodFat m_fat;

		[field: FieldOffset(0)]
		private CorMethodTiny m_tiny;

		internal CorMethodFat* Fat {
			get {
				fixed (CorMethod* value = &this) {
					return &value->m_fat;
				}
			}
		}

		internal CorMethodTiny* Tiny {
			get {
				fixed (CorMethod* value = &this) {
					return &value->m_tiny;
				}
			}
		}

		public ClrStructureType Type => ClrStructureType.Metadata;
	}
}
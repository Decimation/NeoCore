using System;
using System.Runtime.InteropServices;
using Memkit;
using Memkit.Pointers;
using NeoCore.Import.Attributes;
using NeoCore.Model;
using NeoCore.Win32.Attributes;


// ReSharper disable InconsistentNaming

namespace NeoCore.CoreClr.VM.Jit
{
	[ImportNamespace]
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
	
	/***********************************************************************************/
	// Legal values for
	// * code:IMAGE_COR_ILMETHOD_FAT::Flags or
	// * code:IMAGE_COR_ILMETHOD_TINY::Flags_CodeSize fields.
	//
	// The only semantic flag at present is CorILMethod_InitLocals
	[Flags]
	public enum CorILMethodFlags : ushort
	{
		/// <summary>
		/// Call default constructor on all local vars
		/// </summary>
		InitLocals = 0x0010,

		/// <summary>
		/// There is another attribute after this one
		/// </summary>
		MoreSects = 0x0008,

		/// <summary>
		/// Not used/
		/// </summary>
		CompressedIL = 0x0040,

		// Indicates the format for the COR_ILMETHOD header
		FormatShift = 3,

		FormatMask = ((1 << FormatShift) - 1),

		/// <summary>
		/// Use this code if the code size is even
		/// </summary>
		TinyFormat = 0x0002,

		SmallFormat = 0x0000,

		FatFormat = 0x0003,

		/// <summary>
		/// Use this code if the code size is odd
		/// </summary>
		TinyFormat1 = 0x0006,
	}
}
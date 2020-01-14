using System;
using System.Runtime.InteropServices;
using NeoCore.Import.Attributes;
using NeoCore.Model;
using NeoCore.Win32.Attributes;

// ReSharper disable InconsistentNaming

namespace NeoCore.CoreClr.VM.EE
{
	[ImportNamespace]
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct EEClassLayoutInfo : IClrStructure
	{
		public ClrStructureType Type => ClrStructureType.Metadata;

		public string NativeName => nameof(EEClassLayoutInfo);

		#region Fields

		// why is there an m_cbNativeSize in EEClassLayoutInfo and EEClass?
		internal int NativeSize { get; }

		internal int ManagedSize { get; }

		// 1,2,4 or 8: this is equal to the largest of the alignment requirements
		// of each of the EEClass's members. If the NStruct extends another NStruct,
		// the base NStruct is treated as the first member for the purpose of
		// this calculation.

		// Alias: LargestAlignmentRequirementOfAllMembers
		internal byte MaxAlignReqOfAll { get; }

		// Post V1.0 addition: This is the equivalent of m_LargestAlignmentRequirementOfAllMember
		// for the managed layout.

		// Alias: ManagedLargestAlignmentRequirementOfAllMembers
		internal byte ManagedMaxAlignReqOfAll { get; }

		internal LayoutFlags Flags { get; }

		internal byte PackingSize { get; }

		/// <summary>
		///     # of fields that are of the calltime-marshal variety.
		/// </summary>
		internal int NumCTMFields { get; }

		internal void* FieldMarshalers { get; }

		#endregion
	}
	
	
	[Flags]
	public enum LayoutFlags : byte
	{
		/// <summary>
		///     TRUE if the GC layout of the class is bit-for-bit identical
		///     to its unmanaged counterpart (i.e. no internal reference fields,
		///     no ansi-unicode char conversions required, etc.) Used to
		///     optimize marshaling.
		/// </summary>
		Blittable = 0x01,

		/// <summary>
		///     Is this type also sequential in managed memory?
		/// </summary>
		ManagedSequential = 0x02,

		/// <summary>
		///     When a sequential/explicit type has no fields, it is conceptually
		///     zero-sized, but actually is 1 byte in length. This holds onto this
		///     fact and allows us to revert the 1 byte of padding when another
		///     explicit type inherits from this type.
		/// </summary>
		ZeroSized = 0x04,

		/// <summary>
		///     The size of the struct is explicitly specified in the meta-data.
		/// </summary>
		HasExplicitSize = 0x08,

		/// <summary>
		///     Whether a native struct is passed in registers.
		/// </summary>
		NativePassInRegisters = 0x10,

		R4HFA = 0x10,
		R8HFA = 0x20
	}
}
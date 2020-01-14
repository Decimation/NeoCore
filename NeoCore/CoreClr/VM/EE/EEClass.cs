using System;
using System.Reflection;
using System.Runtime.InteropServices;
using Memkit.Pointers;
using Memkit.Pointers.ExtraPointers;
using NeoCore.Import.Attributes;
using NeoCore.Model;
using NeoCore.Utilities;
using NeoCore.Win32.Attributes;

// ReSharper disable ConvertToAutoPropertyWhenPossible

// ReSharper disable InconsistentNaming

namespace NeoCore.CoreClr.VM.EE
{
	[ImportNamespace]
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct EEClass : IClrStructure
	{
		public ClrStructureType Type => ClrStructureType.Metadata;

		public string NativeName => nameof(EEClass);

		#region Fields

		internal void* GuidInfo { get; }

		internal void* OptionalFields { get; }

		internal MethodTable* MethodTable { get; }

		/// <summary>
		/// <see cref="RelativePointer{T}"/> to <see cref="FieldDesc"/>
		/// </summary>
		private FieldDesc* FieldDescList { get; }

		internal void* Chunks { get; }

		#region Union 1

		/// <summary>
		///     <para>Union 1</para>
		///     <para><see cref="ObjectHandleDelegate" /></para>
		///     <para><see cref="NativeSize" /></para>
		///     <para><see cref="InterfaceType" /></para>
		/// </summary>
		private void* Union1 { get; }

		internal void* ObjectHandleDelegate => Union1;

		internal uint NativeSize {
			get {
				var ul = (ulong) Union1;
				return (uint) ul;
			}
		}

		internal CorInterfaceType InterfaceType {
			get {
				var ul = (ulong) Union1;
				return (CorInterfaceType) (uint) ul;
			}
		}

		#endregion


		internal void* CCWTemplate { get; }

		internal TypeAttributes Attributes { get; }

		internal VMFlags VMFlags { get; }

		internal CorElementType NormType { get; }

		internal bool FieldsArePacked { get; }

		internal byte FixedEEClassFields { get; }

		internal byte BaseSizePadding { get; }

		#endregion

		#region Accessors

		internal Pointer<FieldDesc> FieldList {
			get {
				const int FD_LIST_FIELD_OFFSET = 24;

				return (FieldDesc*) Structures.FieldOffsetAlt(ref this, FD_LIST_FIELD_OFFSET, FieldDescList);
			}
		}

		internal Pointer<ArrayClass> AsArrayClass {
			get {
				fixed (EEClass* value = &this) {
					return Structures.ReadSubStructure<EEClass, ArrayClass>(value);
				}
			}
		}

		internal CorElementType ArrayElementType => AsArrayClass.Reference.ElementType;

		internal byte ArrayRank => AsArrayClass.Reference.Rank;

		internal Pointer<EEClassLayoutInfo> LayoutInfo {
			get {
				fixed (EEClass* value = &this) {
					return Structures.ReadSubStructure<EEClass, LayoutEEClass>(value)
					              .Cast<EEClassLayoutInfo>();
				}
			}
		}

		/// <summary>
		///     <see cref="FieldDesc"/> list length
		/// </summary>
		internal int FieldListLength {
			get {
				Pointer<EEClass>     pClass    = MethodTable->EEClass;
				Pointer<MethodTable> pParentMT = MethodTable->Parent;

				int fieldCount = pClass.Reference.NumInstanceFields + pClass.Reference.NumStaticFields;

				if (!pParentMT.IsNull)
					fieldCount -= pParentMT.Reference.EEClass.Reference.NumInstanceFields;

				return fieldCount;
			}
		}

		#endregion

		#region Packed fields

		internal int NumInstanceFields => GetPackableField(EEClassFieldId.NumInstanceFields);

		internal int NumStaticFields => GetPackableField(EEClassFieldId.NumStaticFields);

		internal int NumMethods => GetPackableField(EEClassFieldId.NumMethods);

		internal int NumNonVirtualSlots => GetPackableField(EEClassFieldId.NumNonVirtualSlots);

		private int GetPackableField(EEClassFieldId eField)
		{
			var u  = (uint) eField;
			var pf = new PackedFieldsReader(PackedFields, (int) EEClassFieldId.COUNT);
			return (int) (FieldsArePacked ? pf.GetPackedField(u) : pf.GetUnpackedField(u));
		}

		private Pointer<byte> PackedFields {
			get {
				fixed (EEClass* value = &this) {
					var thisptr = (Pointer<byte>) value;
					return thisptr.Add(FixedEEClassFields);
				}
			}
		}

		#endregion
	}

	[StructLayout(LayoutKind.Explicit)]
	internal struct LayoutEEClass : IClrStructure, INativeSubclass<EEClass>
	{
		// Note: This offset should be 72 or sizeof(EEClass)
		// 		 but I'm keeping it at 0 to minimize size usage,
		//		 so I'll just offset the pointer by 72 bytes
		[field: FieldOffset(0)]
		internal EEClassLayoutInfo LayoutInfo { get; }

		public ClrStructureType Type => ClrStructureType.Metadata;

		public string NativeName => nameof(LayoutEEClass);
	}
	
	public enum EEClassFieldId : uint
	{
		NumInstanceFields = 0,
		NumMethods,
		NumStaticFields,
		NumHandleStatics,
		NumBoxedStatics,
		NonGCStaticFieldBytes,
		NumThreadStaticFields,
		NumHandleThreadStatics,
		NumBoxedThreadStatics,
		NonGCThreadStaticFieldBytes,
		NumNonVirtualSlots,
		COUNT
	}
	
	
	[Flags]
	public enum VMFlags : uint
	{
		LayoutDependsOnOtherModules = 0x00000001,
		Delegate                    = 0x00000002,

		/// <summary>
		///     Value type Statics in this class will be pinned
		/// </summary>
		FixedAddressVtStatics = 0x00000020,
		HasLayout        = 0x00000040,
		IsNested         = 0x00000080,
		IsEquivalentType = 0x00000200,

		//   OVERLAYED is used to detect whether Equals can safely optimize to a bit-compare across the structure.
		HasOverlayedFields = 0x00000400,

		// Set this if this class or its parent have instance fields which
		// must be explicitly inited in a constructor (e.g. pointers of any
		// kind, gc or native).
		//
		// Currently this is used by the verifier when verifying value classes
		// - it's ok to use uninitialised value classes if there are no
		// pointer fields in them.
		HasFieldsWhichMustBeInited = 0x00000800,

		UnsafeValueType = 0x00001000,

		/// <summary>
		///     <see cref="BestFitMapping" /> and <see cref="ThrowOnUnmappableChar" /> are valid only if this is set
		/// </summary>
		BestFitMappingInited = 0x00002000,
		BestFitMapping        = 0x00004000, // BestFitMappingAttribute.Value
		ThrowOnUnmappableChar = 0x00008000, // BestFitMappingAttribute.ThrowOnUnmappableChar

		// unused                              = 0x00010000,
		NoGuid             = 0x00020000,
		HasNonPublicFields = 0x00040000,

		// unused                              = 0x00080000,
		ContainsStackPtr = 0x00100000,

		/// <summary>
		///     Would like to have 8-byte alignment
		/// </summary>
		PreferAlign8 = 0x00200000,
		// unused                              = 0x00400000,

		SparseForCominterop = 0x00800000,

		// interfaces may have a coclass attribute
		HasCoClassAttrib   = 0x01000000,
		ComEventItfMask    = 0x02000000, // class is a special COM event interface
		ProjectedFromWinRT = 0x04000000,
		ExportedToWinRT    = 0x08000000,

		// This one indicates that the fields of the valuetype are
		// not tightly packed and is used to check whether we can
		// do bit-equality on value types to implement ValueType::Equals.
		// It is not valid for classes, and only matters if ContainsPointer
		// is false.
		NotTightlyPacked = 0x10000000,

		// True if methoddesc on this class have any real (non-interface) methodimpls
		ContainsMethodImpls        = 0x20000000,
		MarshalingTypeMask         = 0xc0000000,
		MarshalingTypeInhibit      = 0x40000000,
		MarshalingTypeFreeThreaded = 0x80000000,
		MarshalingTypeStandard     = 0xc0000000
	}
}
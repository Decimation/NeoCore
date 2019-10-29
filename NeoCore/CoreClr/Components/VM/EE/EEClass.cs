using System.Reflection;
using System.Runtime.InteropServices;
using NeoCore.CoreClr.Components.Support;
using NeoCore.CoreClr.Meta.Base;
using NeoCore.Import.Attributes;
using NeoCore.Interop.Attributes;
using NeoCore.Memory.Pointers;

// ReSharper disable ConvertToAutoPropertyWhenPossible

// ReSharper disable InconsistentNaming

namespace NeoCore.CoreClr.Components.VM.EE
{
	[ImportNamespace]
	[NativeStructure]
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct EEClass : IClrStructure
	{
		
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
		private void* m_union1;

		internal void* ObjectHandleDelegate => m_union1;

		internal uint NativeSize {
			get {
				fixed (EEClass* value = &this) {
					Pointer<uint> ptr = &value->m_union1;
					return ptr.Reference;
				}
			}
		}

		internal CorInterfaceType InterfaceType {
			get {
				fixed (EEClass* value = &this) {
					Pointer<int> ptr = &value->m_union1;
					return (CorInterfaceType) ptr.Reference;
				}
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
				
				return (FieldDesc*) ClrAccess.FieldOffsetAlt(ref this, FD_LIST_FIELD_OFFSET, FieldDescList);
			}
		}

		internal Pointer<ArrayClass> AsArrayClass {
			get {
				fixed (EEClass* value = &this) {
					return Runtime.ReadSubStructure<EEClass, ArrayClass>(value);
				}
			}
		}

		internal CorElementType ArrayElementType => AsArrayClass.Reference.ElementType;
		
		internal byte ArrayRank => AsArrayClass.Reference.Rank;
		
		internal Pointer<EEClassLayoutInfo> LayoutInfo {
			get {
				fixed (EEClass* value = &this) {
					return Runtime.ReadSubStructure<EEClass, LayoutEEClass>(value)
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

		internal int NumInstanceFields  => GetPackableField(EEClassFieldId.NumInstanceFields);
		
		internal int NumStaticFields    => GetPackableField(EEClassFieldId.NumStaticFields);
		
		internal int NumMethods         => GetPackableField(EEClassFieldId.NumMethods);
		
		internal int NumNonVirtualSlots => GetPackableField(EEClassFieldId.NumNonVirtualSlots);

		private int GetPackableField(EEClassFieldId eField)
		{
			var u = (uint) eField;
			var pf = new ClrPackedFieldsReader(PackedFields,(int) EEClassFieldId.COUNT);
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

		public ClrStructureType Type => ClrStructureType.Metadata;
	}
	
	[StructLayout(LayoutKind.Explicit)]
	internal struct LayoutEEClass : IClrStructure, INativeInheritance<EEClass>
	{
		// Note: This offset should be 72 or sizeof(EEClass)
		// 		 but I'm keeping it at 0 to minimize size usage,
		//		 so I'll just offset the pointer by 72 bytes
		[FieldOffset(0)]
		internal EEClassLayoutInfo m_LayoutInfo;
		
		public ClrStructureType Type => ClrStructureType.Metadata;
	}
}
using System;
using System.Reflection;
using System.Runtime.InteropServices;
using NeoCore.Assets;
using NeoCore.CoreClr.Support;
using NeoCore.Import.Attributes;
using NeoCore.Memory;
using NeoCore.Utilities;

// ReSharper disable ConvertToAutoPropertyWhenPossible

// ReSharper disable InconsistentNaming

namespace NeoCore.CoreClr.Metadata.EE
{
	[ImportNamespace]
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct EEClass
	{
		#region Fields

		internal void* GuidInfo { get; }

		internal void* OptionalFields { get; }

		internal MethodTable* MethodTable { get; }

		private FieldDesc* FieldDescList { get; }

		internal void* Chunks { get; }
		
		#region Union 1

		/// <summary>
		///     <para>Union 1</para>
		///     <para>void* <see cref="ObjectHandleDelegate" /></para>
		///     <para>uint <see cref="NativeSize" /></para>
		///     <para>int <see cref="InterfaceType" /></para>
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

		internal InterfaceType InterfaceType {
			get {
				fixed (EEClass* value = &this) {
					Pointer<int> ptr = &value->m_union1;
					return (InterfaceType) ptr.Reference;
				}
			}
		}

		#endregion


		internal void* CCWTemplate { get; }

		internal TypeAttributes Attributes { get; }

		internal VMFlags VMFlags { get; }

		internal ElementType NormType { get; }

		internal bool FieldsArePacked { get; }

		internal byte FixedEEClassFields { get; }

		internal byte BaseSizePadding { get; }

		#endregion

		#region Accessors

		internal FieldDesc* FieldList {
			get {
				// Offset for the field
				const int FD_LIST_FIELD_OFFSET = 24;
				return (FieldDesc*) Runtime.FieldOffset(ref this, FD_LIST_FIELD_OFFSET, FieldDescList);
			}
		}


		internal Pointer<EEClassLayoutInfo> LayoutInfo {
			get {
				fixed (EEClass* value = &this) {
					void* thisptr = ((Pointer<byte>) value).Add(sizeof(EEClass)).ToPointer();
					return &((LayoutEEClass*) thisptr)->m_LayoutInfo;
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
			return (int) (FieldsArePacked ? PackedFields->GetPackedField(u) : PackedFields->GetUnpackedField(u));
		}

		private PackedFields* PackedFields {
			get {
				fixed (EEClass* value = &this) {
					var thisptr = (Pointer<byte>) value;
					return (PackedFields*) thisptr.Add(FixedEEClassFields);
				}
			}
		}

		#endregion
	}
	
	[StructLayout(LayoutKind.Explicit)]
	internal struct LayoutEEClass
	{
		// Note: This offset should be 72 or sizeof(EEClass)
		// 		 but I'm keeping it at 0 to minimize size usage,
		//		 so I'll just offset the pointer by 72 bytes
		[FieldOffset(0)]
		internal EEClassLayoutInfo m_LayoutInfo;
	}
}
using System;
using System.Reflection;
using NeoCore.CoreClr.Meta.Base;
using NeoCore.CoreClr.Metadata;
using NeoCore.CoreClr.Metadata.EE;
using NeoCore.Memory;
using NeoCore.Utilities;
using NeoCore.Utilities.Diagnostics;

// ReSharper disable InconsistentNaming

namespace NeoCore.CoreClr.Meta
{
	/// <summary>
	///     <list type="bullet">
	///         <item><description>CLR structure: <see cref="MethodTable"/>, <see cref="EEClass"/>, and
	/// 		<see cref="TypeHandle"/></description></item>
	///         <item><description>Reflection structure: <see cref="Type"/></description></item>
	///     </list>
	/// </summary>
	public unsafe class MetaType : ClrStructure<MethodTable>
	{
		#region Constructor

		// Root constructor
		internal MetaType(Pointer<MethodTable> mt) : base(mt)
		{
			RuntimeType = Runtime.ResolveType(mt.Cast());
		}

		public MetaType(Type t) : this(Runtime.ResolveHandle(t)) { }

		#endregion

		#region Accessors

		public override MemberInfo Info => RuntimeType;
		
		

		#region bool

		public bool IsInteger {
			get {
				return Type.GetTypeCode(RuntimeType) switch
				{
					TypeCode.Byte => true,
					TypeCode.SByte => true,
					TypeCode.UInt16 => true,
					TypeCode.Int16 => true,
					TypeCode.UInt32 => true,
					TypeCode.Int32 => true,
					TypeCode.UInt64 => true,
					TypeCode.Int64 => true,
					_ => false
				};
			}
		}

		public bool IsReal {
			get {
				return Type.GetTypeCode(RuntimeType) switch
				{
					TypeCode.Decimal => true,
					TypeCode.Double => true,
					TypeCode.Single => true,
					_ => false
				};
			}
		}

		public bool IsNumeric => IsInteger || IsReal;

		public bool IsStruct => RuntimeType.IsValueType;

		public bool IsPointer => RuntimeType.IsPointer;

//		public bool IsIListType => RuntimeType.IsIListType();

//		public bool IsIEnumerableType => RuntimeType.IsEnumerableType();

		#region Unmanaged

		/// <summary>
		/// Dummy class for use with <see cref="IsUnmanaged"/> and <see cref="IsUnmanaged"/>
		/// </summary>
		// ReSharper disable once UnusedTypeParameter
		private sealed class U<T> where T : unmanaged { }

		/// <summary>
		/// Determines whether this type fits the <c>unmanaged</c> type constraint.
		/// </summary>
		public bool IsUnmanaged {
			get {
				try {
					// ReSharper disable once ReturnValueOfPureMethodIsNotUsed
					typeof(U<>).MakeGenericType(RuntimeType);
					return true;
				}
				catch {
					return false;
				}
			}
		}

		#endregion

		#endregion

		#region MethodTable

		public short ComponentSize => Value.Reference.ComponentSize;

		public GenericsFlags FlagsLow => Value.Reference.Generics;

		public int BaseSize => Value.Reference.BaseSize;

		public SlotsFlags Flags2 => Value.Reference.Slots;

		public override int Token => Tokens.TokenFromRid(Value.Reference.RawToken, CorTokenType.TypeDef);

		public short VirtualsCount => Value.Reference.NumVirtuals;

		public short InterfacesCount => Value.Reference.NumInterfaces;

		public MetaType Parent => (Pointer<MethodTable>) Value.Reference.Parent;

		public Pointer<byte> Module => Value.Reference.Module;

		public Pointer<byte> WriteableData => Value.Reference.WriteableData;

		public TypeHierarchy Flags => Value.Reference.Hierarchy;

		private Pointer<EEClass> EEClass => Value.Reference.EEClass;

		public MetaType Canon => Value.Reference.Canon;

		public Pointer<byte> PerInstInfo => Value.Reference.PerInstInfo;

		public MetaType ElementTypeHandle => (Pointer<MethodTable>) Value.Reference.ElementTypeHandle;

//		public Pointer<byte> MultipurposeSlot1 => Value.Reference.MultipurposeSlot1;

		public Pointer<byte> InterfaceMap => Value.Reference.InterfaceMap;

//		public Pointer<byte> MultipurposeSlot2 => Value.Reference.MultipurposeSlot2;

		public Type RuntimeType { get; }

		#endregion

		#region EEClass

		public Pointer<byte> GuidInfo => EEClass.Reference.GuidInfo;

		public Pointer<byte> OptionalFields => EEClass.Reference.OptionalFields;

		public Pointer<byte> Chunks => EEClass.Reference.Chunks;

		public int NativeSize => (int) EEClass.Reference.NativeSize;

		public CorInterfaceAttr ComInterfaceType => EEClass.Reference.ComInterfaceType;

//		public Pointer<byte> CCWTemplate => EEClass.Reference.CCWTemplate;

		public TypeAttributes Attributes => EEClass.Reference.Attributes;

		public VMFlags VMFlags => EEClass.Reference.VMFlags;

		public CorElementType NormType => EEClass.Reference.NormType;

		public bool FieldsArePacked => EEClass.Reference.FieldsArePacked;

		public int FixedEEClassFields => EEClass.Reference.FixedEEClassFields;

		/// <summary>
		/// Size of the padding in <see cref="BaseSize"/>
		/// </summary>
		public int BaseSizePadding => EEClass.Reference.BaseSizePadding;

		public MetaLayout LayoutInfo {
			get {
				Guard.Assert(HasLayout);

				return new MetaLayout(EEClass.Reference.LayoutInfo);
			}
		}

		public int InstanceFieldsCount => EEClass.Reference.NumInstanceFields;

		/// <summary>
		/// Number of fields that are not <see cref="MetaField.IsLiteral"/> but <see cref="MetaField.IsStatic"/>
		/// </summary>
		public int StaticFieldsCount => EEClass.Reference.NumStaticFields;

		public int MethodsCount => EEClass.Reference.NumMethods;

		public int NonVirtualSlotsCount => EEClass.Reference.NumNonVirtualSlots;

		/// <summary>
		/// Size of instance fields
		/// </summary>
		public int InstanceFieldsSize => BaseSize - BaseSizePadding;

		public int FieldsCount => EEClass.Reference.FieldDescListLength;

		/*public MetaField[] FieldList {
			get {
				var ptr = (Pointer<FieldDesc>) EEClass.Reference.FieldDescList;
				int len = FieldsCount;

				var rg = new MetaField[len];

				for (int i = 0; i < len; i++) {
					rg[i] = new MetaField(ptr.AddressOfIndex(i));
				}

				return rg;
			}
		}*/

		#region bool

		/// <summary>
		///     Whether this <see cref="EEClass" /> has a <see cref="EEClassLayoutInfo" />
		/// </summary>
		public bool HasLayout => VMFlags.HasFlagFast(VMFlags.HasLayout);

		public bool IsDelegate => VMFlags.HasFlagFast(VMFlags.Delegate);

		public bool IsBlittable => HasLayout && LayoutInfo.Flags.HasFlagFast(LayoutFlags.Blittable);

		public bool HasComponentSize => Flags.HasFlagFast(TypeHierarchy.HasComponentSize);

		public bool IsArray => Flags.HasFlagFast(TypeHierarchy.Array);
		
		public bool IsStringOrArray => HasComponentSize;

		public bool IsString => HasComponentSize && !IsArray;
		
		public bool ContainsPointers => Flags.HasFlagFast(TypeHierarchy.ContainsPointers);

		public bool IsReferenceOrContainsReferences => !RuntimeType.IsValueType || ContainsPointers;

		#endregion

		#endregion
		
		#endregion

		#region Operators

		public static implicit operator MetaType(Pointer<MethodTable> ptr) => new MetaType(ptr);

		public static implicit operator MetaType(Type t) => new MetaType(t);

		public bool Equals(MetaType other)
		{
			return base.Equals(other); /*&& RuntimeType == other.RuntimeType*/
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
				return false;

			if (ReferenceEquals(this, obj))
				return true;

			if (obj.GetType() != this.GetType())
				return false;

			return Equals((MetaType) obj);
		}

		public override int GetHashCode()
		{
			unchecked {
				return (base.GetHashCode() * 397) ^ RuntimeType.GetHashCode();
			}
		}

		public static bool operator ==(MetaType left, MetaType right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(MetaType left, MetaType right)
		{
			return !Equals(left, right);
		}

		#endregion
	}
}
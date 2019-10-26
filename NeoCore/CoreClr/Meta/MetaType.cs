using System;
using System.Reflection;
using NeoCore.CoreClr.Meta.Base;
using NeoCore.CoreClr.VM;
using NeoCore.CoreClr.VM.EE;
using NeoCore.Memory;
using NeoCore.Memory.Pointers;
using NeoCore.Utilities;
using NeoCore.Utilities.Diagnostics;
using NeoCore.Utilities.Extensions;
using TypeInfo = NeoCore.CoreClr.VM.TypeInfo;
// ReSharper disable SuggestBaseTypeForParameter

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

		public MetaType(Pointer<MethodTable> mt) : base(mt)
		{
			RuntimeType = Runtime.ResolveType(mt.Cast());
			AuxiliaryProperties = Runtime.Info.ReadProperties(RuntimeType);
		}

		public MetaType(Type t) : this(Runtime.ResolveHandle(t)) { }

		#endregion

		#region Accessors

		protected override Type[] AdditionalSources => new[] {typeof(EEClass), typeof(TypeHandle)};
		
		
		public override MemberInfo Info => RuntimeType;
		
		public AuxiliaryProperties AuxiliaryProperties { get; }

		public bool IsAnyPointer => AuxiliaryProperties.HasFlagFast(AuxiliaryProperties.AnyPointer);
		
		public bool IsReal => AuxiliaryProperties.HasFlagFast(AuxiliaryProperties.Real);
		
		public bool IsNumeric => AuxiliaryProperties.HasFlagFast(AuxiliaryProperties.Numeric);
		
		public bool IsInteger => AuxiliaryProperties.HasFlagFast(AuxiliaryProperties.Integer);
		
		public bool IsEnumerable => AuxiliaryProperties.HasFlagFast(AuxiliaryProperties.Enumerable);
		
		public bool IsStruct => AuxiliaryProperties.HasFlagFast(AuxiliaryProperties.Struct);
		
		public bool IsUnmanaged => AuxiliaryProperties.HasFlagFast(AuxiliaryProperties.Unmanaged);
		
		#region MethodTable

		public short ComponentSize => Value.Reference.ComponentSize;
		
		public int BaseSize => Value.Reference.BaseSize;
		
		public GenericInfo GenericFlags => Value.Reference.GenericFlags;
		
		public OptionalSlots SlotFlags => Value.Reference.SlotFlags;

		public override int Token => Tokens.TokenFromRid(Value.Reference.RawToken, TokenType.TypeDef);

		public short VirtualsCount => Value.Reference.NumVirtuals;

		public short InterfacesCount => Value.Reference.NumInterfaces;

		public MetaType Parent => (Pointer<MethodTable>) Value.Reference.Parent;

		public Pointer<byte> Module => Value.Reference.Module;

		public Pointer<byte> WriteableData => Value.Reference.WriteableData;

		public TypeInfo TypeFlags => Value.Reference.TypeFlags;

		private Pointer<EEClass> EEClass => Value.Reference.EEClass;

		public MetaType Canon => Value.Reference.Canon;

		public Pointer<byte> PerInstInfo => Value.Reference.PerInstInfo;

		public MetaType ElementTypeHandle => (Pointer<MethodTable>) Value.Reference.ElementTypeHandle;

		public Pointer<byte> InterfaceMap => Value.Reference.InterfaceMap;

		public Type RuntimeType { get; }

		#endregion

		#region EEClass

		public Pointer<byte> GuidInfo => EEClass.Reference.GuidInfo;

		public Pointer<byte> OptionalFields => EEClass.Reference.OptionalFields;

		public Pointer<byte> Chunks => EEClass.Reference.Chunks;

		public int NativeSize => (int) EEClass.Reference.NativeSize;

		public InterfaceType InterfaceType => EEClass.Reference.InterfaceType;

		public TypeAttributes Attributes => EEClass.Reference.Attributes;

		public VMFlags VMFlags => EEClass.Reference.VMFlags;

		public ElementType NormType => EEClass.Reference.NormType;

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

		public int FieldsCount => EEClass.Reference.FieldListLength;

		
		public MetaField[] FieldList {
			get {
				// todo: fix

				/*var ptr = (Pointer<FieldDesc>) EEClass.Reference.FieldList;
				int len = FieldsCount;

				var rg = new MetaField[len];

				for (int i = 0; i < len; i++) {
					rg[i] = new MetaField(ptr.AddressOfIndex(i));
				}

				return rg;*/
				
				throw new NotImplementedException();
			}
		}
		
		
		public MetaField this[string name] => GetField(name);
		
		public MetaField GetField(string name) => RuntimeType.GetAnyField(name);
		
		public MetaMethod GetMethod(string name) => RuntimeType.GetAnyMethod(name);
		
//		public MemberInfo[] GetOriginalMember(string name) => RuntimeType.GetAnyMember(name);
		
//		public MemberInfo GetFirstOriginalMember(string name) => GetOriginalMember(name)[0];
		
		#region bool

		/// <summary>
		///     Whether this <see cref="EEClass" /> has a <see cref="EEClassLayoutInfo" />
		/// </summary>
		public bool HasLayout => VMFlags.HasFlagFast(VMFlags.HasLayout);

		public bool IsDelegate => VMFlags.HasFlagFast(VMFlags.Delegate);

		public bool IsBlittable => HasLayout && LayoutInfo.Flags.HasFlagFast(LayoutFlags.Blittable);

		public bool HasComponentSize => TypeFlags.HasFlagFast(TypeInfo.HasComponentSize);

		public bool IsArray => TypeFlags.HasFlagFast(TypeInfo.Array);
		
		public bool IsStringOrArray => HasComponentSize;

		public bool IsString => HasComponentSize && !IsArray;
		
		public bool ContainsPointers => TypeFlags.HasFlagFast(TypeInfo.ContainsPointers);

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
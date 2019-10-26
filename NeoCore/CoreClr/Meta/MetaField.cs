using System;
using System.Reflection;
using NeoCore.CoreClr.Meta.Base;
using NeoCore.CoreClr.VM;
using NeoCore.Memory;
using NeoCore.Memory.Pointers;
using NeoCore.Model;
using NeoCore.Utilities;
using NeoCore.Utilities.Diagnostics;
using NeoCore.Utilities.Extensions;
// ReSharper disable SuggestBaseTypeForParameter

// ReSharper disable InconsistentNaming

namespace NeoCore.CoreClr.Meta
{
	/// <summary>
	///     <list type="bullet">
	///         <item><description>CLR structure: <see cref="FieldDesc"/></description></item>
	///         <item><description>Reflection structure: <see cref="FieldInfo"/></description></item>
	///     </list>
	/// </summary>
	public unsafe class MetaField : EmbeddedClrStructure<FieldDesc>, IStructure
	{
		private const int FIELD_OFFSET_MAX = (1 << 27) - 1;

		private const int FIELD_OFFSET_NEW_ENC = FIELD_OFFSET_MAX - 4;

		#region Constructors

		public MetaField(Pointer<FieldDesc> ptr) : base(ptr) { }

		public MetaField(FieldInfo info) : base(info) { }

		#endregion

		#region Accessors

		public FieldInfo FieldInfo => (FieldInfo) Info;

		public ElementType Element => Value.Reference.Element;

		public AccessModifiers Access => Value.Reference.Access;

		public int Offset => Value.Reference.Offset;

		public override MemberInfo Info => EnclosingType.RuntimeType.Module.ResolveField(Token);

		public MetaType FieldType => FieldInfo.FieldType;

		public override MetaType EnclosingType {
			get { return Value.Reference.ApproxEnclosingMethodTable; }
		}

		public override int Token => Value.Reference.Token;

		public FieldAttributes Attributes => FieldInfo.Attributes;

		#region bool

		public bool IsPointer => Value.Reference.IsPointer;

		public FieldProperties Properties => Value.Reference.Properties;

		public bool IsStatic => Properties.HasFlagFast(FieldProperties.Static);

		public bool IsLiteral => FieldInfo.IsLiteral;

		#endregion

		#region Delegates

		public int Size => Value.Reference.LoadSize();

		public Pointer<byte> GetStaticAddress() => Value.Reference.GetCurrentStaticAddress();

		#endregion

		#endregion

		protected override Type[] AdditionalSources => null;

		public Pointer<byte> GetValueAddress<T>(ref T value)
		{
			return IsStatic ? GetStaticAddress() : GetAddress(ref value);
		}

		public Pointer<byte> GetAddress<T>(ref T value)
		{
			Guard.Assert(!IsStatic, nameof(IsStatic));
			Guard.Assert(Offset != FIELD_OFFSET_NEW_ENC);

			var data = Unsafe.AddressOfFields(ref value) + Offset;

			return data;
		}

		public object GetValue(object value)
		{
			return FieldInfo.GetValue(value);
		}

		public T GetValue<T>(T value)
		{
			return GetAddress(ref value).Cast<T>().Read();
		}


		#region Operators

		public static implicit operator MetaField(Pointer<FieldDesc> ptr) => new MetaField(ptr);

		public static implicit operator MetaField(FieldInfo t) => new MetaField(t);

		#region Equality

//		public static bool operator ==(MetaField left, MetaField right) => Equals(left, right);

//		public static bool operator !=(MetaField left, MetaField right) => !Equals(left, right);

		#endregion

		#endregion
	}
}
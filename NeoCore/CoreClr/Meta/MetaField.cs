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

		protected override Type[] AdditionalSources => null;

		public override ConsoleTable DebugTable {
			get {
				var table = base.DebugTable;

				table.AddRow(nameof(Size), Size);
				table.AddRow(nameof(Offset), Offset);

				return table;
			}
		}

		public Pointer<byte> GetAddress<T>(ref T value)
		{
			Guard.Assert(!IsStatic, nameof(IsStatic));
			Guard.Assert(Offset != FIELD_OFFSET_NEW_ENC);

			Pointer<byte> data = Mem.AddressOfFields(ref value) + Offset;

			return data;
		}

		public object GetValue(object value) => FieldInfo.GetValue(value);

		public Pointer<byte> GetValueAddress<T>(ref T value) => IsStatic ? GetStaticAddress() : GetAddress(ref value);

		public T GetValue<T>(T value) => GetAddress(ref value).Cast<T>().Read();

		#region Constructors

		public MetaField(Pointer<FieldDesc> ptr) : base(ptr) { }

		public MetaField(FieldInfo info) : base(info) { }

		#endregion

		#region Accessors

		public FieldInfo FieldInfo => (FieldInfo) Info;

		public CorElementType Element => Value.Reference.Element;

		public AccessModifiers Access => Value.Reference.Access;

		public int Offset => Value.Reference.Offset;

		public override MemberInfo Info => EnclosingType.RuntimeType.Module.ResolveField(Token);

		public MetaType FieldType => FieldInfo.FieldType;

		public override MetaType EnclosingType => Value.Reference.ApproxEnclosingMethodTable;

		/// <summary>
		/// <remarks>Equals <see cref="System.Reflection.FieldInfo.MetadataToken"/></remarks>
		/// </summary>
		public override int Token => Value.Reference.Token;

		public FieldAttributes Attributes => FieldInfo.Attributes;

		#region bool

		public bool IsPointer => Value.Reference.IsPointer;

		public FieldBitFlags BitFlags => Value.Reference.BitFlags;

		public bool IsStatic => BitFlags.HasFlagFast(FieldBitFlags.Static);

		#endregion

		#region Delegates

		public int Size => Value.Reference.LoadSize();

		/// <summary>
		/// <remarks>Ensure the enclosing type is loaded!</remarks>
		/// </summary>
		public Pointer<byte> GetStaticAddress()
		{
			throw new NotImplementedException();
			//return Value.Reference.GetCurrentStaticAddress();
		}

		#endregion

		#endregion

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
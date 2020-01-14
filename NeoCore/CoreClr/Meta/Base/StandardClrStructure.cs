using System;
using System.Reflection;
using System.Text;
using Memkit.Pointers;
using Memkit.Utilities;
using NeoCore.Memory;
using NeoCore.Model;
using NeoCore.Utilities;

namespace NeoCore.CoreClr.Meta.Base
{
	/// <summary>
	/// Describes a CLR structure that has metadata information.
	/// </summary>
	/// <typeparam name="TClr">CLR structure type (<see cref="IClrStructure"/>)</typeparam>
	public abstract unsafe class StandardClrStructure<TClr> : BasicClrStructure<TClr>
		where TClr : unmanaged, IClrStructure
	{
		#region Fields

		public virtual string Name => Info?.Name;

		public abstract MemberInfo Info { get; }

		public abstract int Token { get; }

		#endregion

		#region Constructors

		internal StandardClrStructure(Pointer<TClr> ptr) : base(ptr) { }

		protected StandardClrStructure(MemberInfo member) : base(member) { }

		#endregion

		#region ToString

		public override ConsoleTable DebugTable {
			get {
				var table = base.DebugTable;

				table.AddRow(nameof(Name), Name);
				table.AddRow(nameof(Info), Info);
				table.AddRow(nameof(Token), Format.ToHexString(Token));

				return table;
			}
		}

		public override string ToString()
		{
			return String.Format("Handle: {0} | Name: {1}", Value, Name);
		}

		#endregion

		#region Equality

		public override bool Equals(object? obj)
		{
			return obj != null && base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return Assets.INVALID_VALUE;
		}

		public static bool operator ==(StandardClrStructure<TClr> left, StandardClrStructure<TClr> right) =>
			Equals(left, right);

		public static bool operator !=(StandardClrStructure<TClr> left, StandardClrStructure<TClr> right) =>
			!Equals(left, right);

		#endregion
	}

	public static class MetaExtensions
	{
		public static MetaType AsMetaType(this Type t) => t;

		public static MetaField AsMetaField(this FieldInfo t) => t;

		public static MetaMethod AsMetaMethod(this MethodInfo t) => t;
	}
}
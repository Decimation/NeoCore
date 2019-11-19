using System;
using System.Reflection;
using System.Text;
using NeoCore.Assets;
using NeoCore.Memory;
using NeoCore.Memory.Pointers;

namespace NeoCore.CoreClr.Meta.Base
{
	/// <summary>
	/// Describes a CLR structure that has metadata information.
	/// </summary>
	/// <typeparam name="TClr">CLR structure type (<see cref="IClrStructure"/>)</typeparam>
	public abstract unsafe class ClrStructure<TClr> : AnonymousClrStructure<TClr> where TClr : unmanaged, IClrStructure
	{
		#region Fields

		public virtual string Name => Info?.Name;

		public abstract MemberInfo Info { get; }

		public abstract int Token { get; }

		#endregion

		#region Constructors

		internal ClrStructure(Pointer<TClr> ptr) : base(ptr) { }

		protected ClrStructure(MemberInfo member) : base(member) { }

		#endregion


		#region ToString

		public override string ToString()
		{
			return String.Format("Handle: {0} | Name: {1}", Value, Name);
		}

		#endregion

		#region Equality

		public override bool Equals(object obj)
		{
			return obj != null && base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return ClrInformation.INVALID_VALUE;
		}

		public static bool operator ==(ClrStructure<TClr> left, ClrStructure<TClr> right) => Equals(left, right);

		public static bool operator !=(ClrStructure<TClr> left, ClrStructure<TClr> right) => !Equals(left, right);

		#endregion
	}
	
	public static class MetaExtensions
	{
		public static MetaType AsMetaType(this Type t) => t;

		public static MetaField AsMetaField(this FieldInfo t) => t;

		public static MetaMethod AsMetaMethod(this MethodInfo t) => t;
	}
}
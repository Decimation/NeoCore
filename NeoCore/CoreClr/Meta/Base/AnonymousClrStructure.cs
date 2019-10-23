using System.Reflection;
using System.Text;
using NeoCore.Assets;
using NeoCore.Memory;

namespace NeoCore.CoreClr.Meta.Base
{
	/// <summary>
	/// Describes a CLR structure that doesn't have an accompanying token or <see cref="MemberInfo"/>
	/// </summary>
	/// <typeparam name="TClr">CLR structure type</typeparam>
	public abstract unsafe class AnonymousClrStructure<TClr> where TClr : unmanaged
	{
		#region Fields

		/// <summary>
		/// Points to the internal CLR structure representing this instance
		/// </summary>
		public Pointer<TClr> Value { get; }

		/// <summary>
		/// The native, built-in form of <see cref="Value"/>
		/// </summary>
		protected internal TClr* NativePointer => Value.ToPointer<TClr>();

		#endregion

		#region Constructors

		// Root constructor
		protected AnonymousClrStructure(Pointer<TClr> ptr)
		{
			Value = ptr;
		}

		internal AnonymousClrStructure(MemberInfo member) : this(Runtime.ResolveHandle(member)) { }

		#endregion


		#region ToString

		public override string ToString()
		{
			var sb = new StringBuilder();

			sb.AppendFormat("Handle: {0}", Value);

			return sb.ToString();
		}

		#endregion

		#region Equality

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
				return false;

			if (ReferenceEquals(this, obj))
				return true;

			if (obj.GetType() != this.GetType())
				return false;

			return Equals((ClrStructure<TClr>) obj);
		}

		public bool Equals(ClrStructure<TClr> other)
		{
			return this.Value == other.Value;
		}

		public override int GetHashCode()
		{
			return Constants.INVALID_VALUE;
		}

		public static bool operator ==(AnonymousClrStructure<TClr> left, AnonymousClrStructure<TClr> right) => Equals(left, right);

		public static bool operator !=(AnonymousClrStructure<TClr> left, AnonymousClrStructure<TClr> right) => !Equals(left, right);

		#endregion
	}
}
using System.Reflection;
using System.Text;
using NeoCore.Assets;
using NeoCore.Memory;

namespace NeoCore.CoreClr.Meta.Base
{
	/// <summary>
	/// Describes a CLR structure that has metadata information.
	/// </summary>
	/// <typeparam name="TClr">CLR structure type</typeparam>
	public abstract unsafe class ClrStructure<TClr> : AnonymousClrStructure<TClr> where TClr : unmanaged
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
			var sb = new StringBuilder();

			sb.AppendFormat("Name: {0}\n", Name);
			sb.AppendFormat("Handle: {0}", Value);

			return sb.ToString();
		}

		#endregion

		#region Equality

		public override bool Equals(object obj)
		{
			if (obj == null) {
				return false;
			}
			
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return Constants.INVALID_VALUE;
		}
		
		public static bool operator ==(ClrStructure<TClr> left, ClrStructure<TClr> right) => Equals(left, right);

		public static bool operator !=(ClrStructure<TClr> left, ClrStructure<TClr> right) => !Equals(left, right);

		#endregion
	}
}
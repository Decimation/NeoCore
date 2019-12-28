using System;
using System.Reflection;
using System.Text;
using NeoCore.Import;
using NeoCore.Memory;
using NeoCore.Memory.Pointers;
using NeoCore.Model;
using NeoCore.Support;

namespace NeoCore.CoreClr.Meta.Base
{
	/// <summary>
	/// Describes a CLR structure that doesn't have an accompanying token or <see cref="MemberInfo"/>
	/// </summary>
	/// <typeparam name="TClr">CLR structure type</typeparam>
	public abstract unsafe class BasicClrStructure<TClr> : IWrapper<TClr> where TClr : unmanaged, IClrStructure
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

		/// <summary>
		/// Root constructor
		/// </summary>
		/// <param name="ptr">Metadata structure handle</param>
		protected BasicClrStructure(Pointer<TClr> ptr)
		{
			Value = ptr;

			LoadSources();
		}

		protected BasicClrStructure(MemberInfo member) : this(Runtime.ResolveHandle(member)) { }

		#endregion

		/// <summary>
		/// Additional <see cref="IClrStructure"/> metadata sources.
		/// </summary>
		protected abstract Type[] AdditionalSources { get; }

		private void LoadSources()
		{
			ImportManager.Value.Load(typeof(TClr), Resources.Clr.Imports);

			if (AdditionalSources != null && AdditionalSources.Length > 0) {
				ImportManager.Value.LoadAll(AdditionalSources, Resources.Clr.Imports);
			}
		}


		#region ToString

		public override string ToString()
		{
			return String.Format("Handle: {0}", Value);
		}

		#endregion

		#region Equality

		public override bool Equals(object? obj)
		{
			if (ReferenceEquals(null, obj))
				return false;

			if (ReferenceEquals(this, obj))
				return true;

			if (obj.GetType() != this.GetType())
				return false;

			return Equals((StandardClrStructure<TClr>) obj);
		}

		public bool Equals(StandardClrStructure<TClr> other)
		{
			return this.Value == other.Value;
		}

		public override int GetHashCode()
		{
			return Assets.INVALID_VALUE;
		}

		public static bool operator ==(BasicClrStructure<TClr> left, BasicClrStructure<TClr> right) =>
			Equals(left, right);

		public static bool operator !=(BasicClrStructure<TClr> left, BasicClrStructure<TClr> right) =>
			!Equals(left, right);

		#endregion
	}
}
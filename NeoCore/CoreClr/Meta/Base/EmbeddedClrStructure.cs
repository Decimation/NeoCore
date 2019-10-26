using System.Reflection;
using NeoCore.CoreClr.Components.VM;
using NeoCore.Memory;
using NeoCore.Memory.Pointers;

namespace NeoCore.CoreClr.Meta.Base
{
	/// <summary>
	/// Describes a <see cref="ClrStructure{TClr}"/> that is enclosed by an accompanying <see cref="MethodTable"/>
	/// </summary>
	/// <typeparam name="TClr">CLR structure type</typeparam>
	public abstract unsafe class EmbeddedClrStructure<TClr> : ClrStructure<TClr>  where TClr : unmanaged, IClrStructure
	{
		#region Constructors

		protected EmbeddedClrStructure(Pointer<TClr> ptr) : base(ptr) { }

		protected EmbeddedClrStructure(MemberInfo info) : base(info) { }

		#endregion

		#region MethodTable

		public abstract MetaType EnclosingType { get; }

		#endregion
	}
}
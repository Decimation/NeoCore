using NeoCore.CoreClr.Metadata;
using NeoCore.Memory;

namespace NeoCore.CoreClr.Meta.Base
{
	/// <summary>
	/// Describes a <see cref="ClrStructure{TClr}"/> that is enclosed by an accompanying <see cref="MethodTable"/>
	/// </summary>
	/// <typeparam name="TClr">CLR structure type</typeparam>
	public abstract unsafe class EmbeddedClrStructure<TClr> : ClrStructure<TClr> where TClr : unmanaged
	{
		#region Constructors

		protected EmbeddedClrStructure(Pointer<TClr> ptr) : base(ptr) { }

		#endregion
		
		#region MethodTable

		public abstract MetaType EnclosingType { get; }

		#endregion
	}
}
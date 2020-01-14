using System.Reflection;
using Memkit.Pointers;
using NeoCore.CoreClr.VM;
using NeoCore.Model;
using NeoCore.Utilities;
using SimpleCore;
using SimpleCore.Formatting;

namespace NeoCore.CoreClr.Meta.Base
{
	/// <summary>
	/// Describes a <see cref="StandardClrStructure{TClr}"/> that is enclosed by an accompanying <see cref="MethodTable"/>
	/// </summary>
	/// <typeparam name="TClr">CLR structure type</typeparam>
	public abstract unsafe class EmbeddedClrStructure<TClr> : StandardClrStructure<TClr> where TClr : unmanaged, IClrStructure
	{
		#region MethodTable

		public abstract MetaType EnclosingType { get; }

		#endregion

		public override ConsoleTable DebugTable {
			get {
				var table = base.DebugTable;

				table.AddRow(nameof(EnclosingType), EnclosingType);

				return table;
			}
		}

		#region Constructors

		protected EmbeddedClrStructure(Pointer<TClr> ptr) : base(ptr) { }

		protected EmbeddedClrStructure(MemberInfo info) : base(info) { }

		#endregion
	}
}
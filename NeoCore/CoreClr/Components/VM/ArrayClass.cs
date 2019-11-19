using System.Runtime.InteropServices;
using NeoCore.Assets;
using NeoCore.CoreClr.Components.VM.EE;
using NeoCore.CoreClr.Meta.Base;
using NeoCore.Import.Attributes;
using NeoCore.Interop.Attributes;

namespace NeoCore.CoreClr.Components.VM
{
	[ImportNamespace]
	[NativeStructure]
	[StructLayout(LayoutKind.Sequential)]
	public struct ArrayClass : IClrStructure, INativeInheritance<EEClass>
	{
		internal byte Rank { get; }

		/// <summary>
		/// Cache of <see cref="MethodTable.ElementTypeHandle"/>
		/// </summary>
		internal CorElementType ElementType { get; }

		public ClrStructureType Type => ClrStructureType.Metadata;
	}
}
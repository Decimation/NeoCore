using System.Runtime.InteropServices;
using NeoCore.CoreClr.Components.Support;
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

		// todo: Doesn't seem to be correct
		
		internal CorElementType ElementType { get; }

		public ClrStructureType Type => ClrStructureType.Metadata;
	}
}
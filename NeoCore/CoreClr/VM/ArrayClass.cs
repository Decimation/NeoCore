using System.Runtime.InteropServices;
using NeoCore.Assets;
using NeoCore.CoreClr.VM.EE;
using NeoCore.Import.Attributes;
using NeoCore.Interop.Attributes;
using NeoCore.Model;

namespace NeoCore.CoreClr.VM
{
	[ImportNamespace]
	[NativeStructure]
	[StructLayout(LayoutKind.Sequential)]
	public struct ArrayClass : IClrStructure, INativeSubclass<EEClass>
	{
		internal byte Rank { get; }

		/// <summary>
		/// Cache of <see cref="MethodTable.ElementTypeHandle"/>
		/// </summary>
		internal CorElementType ElementType { get; }

		public ClrStructureType Type => ClrStructureType.Metadata;

		public string NativeName => nameof(ArrayClass);
	}
}
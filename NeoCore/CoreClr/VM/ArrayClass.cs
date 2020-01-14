using System.Runtime.InteropServices;
using NeoCore.CoreClr.VM.EE;
using NeoCore.Import.Attributes;
using NeoCore.Model;
using NeoCore.Utilities;
using NeoCore.Win32.Attributes;

namespace NeoCore.CoreClr.VM
{
	[ImportNamespace]
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
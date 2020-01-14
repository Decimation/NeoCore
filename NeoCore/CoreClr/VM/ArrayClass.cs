using System.Runtime.InteropServices;
using Memkit.Model;
using NeoCore.CoreClr.VM.EE;
using NeoCore.Import.Attributes;
using NeoCore.Model;
using NeoCore.Utilities;

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
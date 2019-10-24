using System.Runtime.InteropServices;
using NeoCore.Assets;
using NeoCore.CoreClr.Support;
using NeoCore.Import;
using NeoCore.Import.Attributes;
using NeoCore.Interop;
using NeoCore.Interop.Attributes;
using NeoCore.Memory;
using NeoCore.Memory.Pointers;

namespace NeoCore.CoreClr.Metadata
{
	[ImportNamespace]
	[NativeStructure]
	[StructLayout(LayoutKind.Sequential)]
	internal unsafe struct TypeHandle : IClr
	{
		private void* Union1 { get; }
		
		private MethodTable* AsMethodTable => (MethodTable*) Union1;

		[ImportMapField]
		private static readonly ImportMap Imports = new ImportMap();

		internal Pointer<MethodTable> MethodTable {
			[ImportProperty]
			get {
				fixed (TypeHandle* value = &this) {
					return Functions.Native.CallReturnPointer((void*) Imports[nameof(MethodTable)], value);
				}
			}
		}
	}
}
using System.Runtime.InteropServices;
using NeoCore.Assets;
using NeoCore.CoreClr.Meta.Base;
using NeoCore.Import;
using NeoCore.Import.Attributes;
using NeoCore.Interop;
using NeoCore.Interop.Attributes;
using NeoCore.Memory.Pointers;

namespace NeoCore.CoreClr.Components.VM
{
	[ImportNamespace]
	[NativeStructure]
	[StructLayout(LayoutKind.Sequential)]
	internal unsafe struct TypeHandle : IClrStructure
	{
		static TypeHandle()
		{
			ImportManager.Value.Load(typeof(TypeHandle), Resources.Clr.Imports);
		}

		private void* Union1 { get; }

		private MethodTable* AsMethodTable => (MethodTable*) Union1;
		
		private ulong AsTAddr => (ulong) Union1;
		
		[ImportMapField]
		private static readonly ImportMap Imports = new ImportMap();

		internal Pointer<MethodTable> MethodTable {
			[ImportAccessor]
			get {
				fixed (TypeHandle* value = &this) {
					return Functions.Native.CallReturnPointer((void*) Imports[nameof(MethodTable)], value);
				}
			}
		}

//		internal bool IsTypeDesc => (AsTAddr & 2) != 0;
		
		public ClrStructureType Type => ClrStructureType.Metadata;
	}
}
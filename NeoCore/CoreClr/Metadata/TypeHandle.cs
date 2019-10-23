using System.Runtime.InteropServices;
using NeoCore.Assets;
using NeoCore.Import;
using NeoCore.Import.Attributes;
using NeoCore.Interop;
using NeoCore.Interop.Attributes;
using NeoCore.Memory;

namespace NeoCore.CoreClr.Metadata
{
	
	[ImportNamespace]
	[StructLayout(LayoutKind.Explicit)]
	internal unsafe struct TypeHandle
	{
		static TypeHandle()
		{
			ImportManager.Value.Load(typeof(TypeHandle), Resources.Clr.Imports);
		}

		#region Fields

		[field: FieldOffset(default)]
		private void* Union1 { get; }

		#endregion

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
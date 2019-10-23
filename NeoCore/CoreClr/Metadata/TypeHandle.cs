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

//		[FieldOffset(default)]
//		private TAddr m_asTAddr;

		[FieldOffset(default)]
		private void* m_asPtr;

		[FieldOffset(default)]
		private MethodTable* m_asMT;

		#endregion

		[ImportMapField]
		private static readonly ImportMap Imports = new ImportMap();
		
		internal Pointer<MethodTable> MethodTable {
			[ImportCall(IdentifierOptions.UseAccessorName, ImportCallOptions.Map)]
			get {
				fixed (TypeHandle* value = &this) {
					return Functions.Native.CallReturnPointer((void*) Imports[nameof(MethodTable)], value);
				}
			}
		}
	}
}
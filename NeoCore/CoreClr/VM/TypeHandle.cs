using System;
using System.Runtime.InteropServices;
using Memkit;
using Memkit.Pointers;
using NeoCore.Import;
using NeoCore.Import.Attributes;
using NeoCore.Model;
using NeoCore.Support;

namespace NeoCore.CoreClr.VM
{
	[ImportNamespace]
	[StructLayout(LayoutKind.Sequential)]
	internal unsafe struct TypeHandle : IClrStructure
	{
		[ImportMapField]
		private static readonly ImportMap Imports = new ImportMap();

		static TypeHandle()
		{
			//ImportManager.Value.Load(typeof(TypeHandle), Resources.Clr.Imports);
		}

		private void* Union1 { get; }

		private MethodTable* AsMethodTable => (MethodTable*) Union1;

		private ulong AsTAddr => (ulong) Union1;

		internal Pointer<MethodTable> MethodTable {
			[ImportAccessor]
			get {
				fixed (TypeHandle* value = &this) {
					return Imports.CallReturnPointer(nameof(MethodTable), (ulong) value);
				}
			}
		}

//		internal bool IsTypeDesc => (AsTAddr & 2) != 0;

		public ClrStructureType Type => ClrStructureType.Metadata;

		public string NativeName => nameof(TypeHandle);
	}
}
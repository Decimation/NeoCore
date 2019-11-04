using System.Runtime.InteropServices;
using NeoCore.CoreClr.Components.Support;
using NeoCore.CoreClr.Meta.Base;
using NeoCore.Import;
using NeoCore.Import.Attributes;
using NeoCore.Interop;
using NeoCore.Interop.Attributes;
using NeoCore.Memory;
using NeoCore.Memory.Pointers;
using NeoCore.Utilities.Extensions;

// ReSharper disable UnassignedGetOnlyAutoProperty
// ReSharper disable InconsistentNaming

namespace NeoCore.CoreClr.Components.VM
{
	[ImportNamespace]
	[NativeStructure]
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct FieldDesc : IClrStructure
	{
		[ImportMapField]
		private static readonly ImportMap Imports = new ImportMap();

		#region Fields

		/// <summary>
		/// <see cref="RelativePointer{T}"/> to <see cref="MethodTable"/>
		/// </summary>
		private RelativePointer<MethodTable> EnclosingMethodTableStub { get; }

		/// <summary>
		/// <c>DWORD</c> #1
		///     <para>unsigned m_mb : 24;</para>
		///     <para>unsigned m_isStatic : 1;</para>
		///     <para>unsigned m_isThreadLocal : 1;</para>
		///     <para>unsigned m_isRVA : 1;</para>
		///     <para>unsigned m_prot : 3;</para>
		///     <para>unsigned m_requiresFullMbValue : 1;</para>
		/// </summary>
		private uint UInt1 { get; }

		/// <summary>
		/// <c>DWORD</c> #2
		///     <para>unsigned m_dwOffset : 27;</para>
		///     <para>unsigned m_type : 5;</para>
		/// </summary>
		private uint UInt2 { get; }

		#endregion

		#region Calculated values

		internal int Token {
			get {
				var rawToken = (int) (UInt1 & 0xFFFFFF);
				// Check if this FieldDesc is using the packed mb layout
				if (!BitFlags.HasFlagFast(FieldBitFlags.RequiresFullMBValue))
					return ClrSigs.TokenFromRid(rawToken & (int) PackedLayoutMask.MBMask, CorTokenType.FieldDef);

				return ClrSigs.TokenFromRid(rawToken, CorTokenType.FieldDef);
			}
		}

		internal int Offset => (int) (UInt2 & 0x7FFFFFF);

		internal CorElementType Element => (CorElementType) (int) ((UInt2 >> 27) & 0x7FFFFFF);

		internal AccessModifiers Access => (AccessModifiers) (int) ((UInt1 >> 26) & 0x3FFFFFF);

		internal bool IsPointer => Element == CorElementType.Ptr;

		internal FieldBitFlags BitFlags => (FieldBitFlags) UInt1;

		#endregion

		#region Imports

		[ImportCall(ImportCallOptions.Map)]
		internal int LoadSize()
		{
			fixed (FieldDesc* value = &this) {
				return Imports.Call<int, ulong>(nameof(LoadSize), (ulong) value);
			}
		}

		[ImportCall(ImportCallOptions.Map)]
		internal void* GetCurrentStaticAddress()
		{
			fixed (FieldDesc* value = &this) {
				return Imports.CallReturnPointer(nameof(GetCurrentStaticAddress), (ulong) value);
			}
		}

		#endregion

		internal Pointer<MethodTable> ApproxEnclosingMethodTable {
			get {
				// m_pMTOfEnclosingClass.GetValue(PTR_HOST_MEMBER_TADDR(FieldDesc, this, m_pMTOfEnclosingClass));

				const int MT_FIELD_OFS = 0;

				return Structures.FieldOffset(EnclosingMethodTableStub.NativeValue, MT_FIELD_OFS);
			}
		}

		public ClrStructureType Type => ClrStructureType.Metadata;
	}
}
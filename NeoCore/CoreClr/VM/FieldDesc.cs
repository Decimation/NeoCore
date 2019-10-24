using System.Runtime.InteropServices;
using NeoCore.CoreClr.Support;
using NeoCore.Import;
using NeoCore.Import.Attributes;
using NeoCore.Interop;
using NeoCore.Interop.Attributes;
using NeoCore.Memory.Pointers;
using NeoCore.Utilities;

// ReSharper disable UnassignedGetOnlyAutoProperty
// ReSharper disable InconsistentNaming

namespace NeoCore.CoreClr.VM
{
	[ImportNamespace]
	[NativeStructure]
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct FieldDesc : IClr
	{
		[ImportMapField]
		private static readonly ImportMap Imports = new ImportMap();

		#region Fields

		/// <summary>
		/// <see cref="RelativePointer{T}"/> to <see cref="MethodTable"/>
		/// </summary>
		private RelativePointer<byte> EnclosingMethodTableStub { get; }

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
				if (!Properties.HasFlagFast(FieldProperties.RequiresFullMBValue))
					return Tokens.TokenFromRid(rawToken & (int) PackedLayoutMask.MBMask, TokenType.FieldDef);

				return Tokens.TokenFromRid(rawToken, TokenType.FieldDef);
			}
		}

		internal int Offset => (int) (UInt2 & 0x7FFFFFF);

		internal ElementType Element => (ElementType) (int) ((UInt2 >> 27) & 0x7FFFFFF);

		internal AccessModifiers Access => (AccessModifiers) (int) ((UInt1 >> 26) & 0x3FFFFFF);

		internal bool IsPointer => Element == ElementType.Ptr;

		internal FieldProperties Properties => (FieldProperties) UInt1;

		#endregion

		#region Imports

		[ImportCall(ImportCallOptions.Map)]
		internal int LoadSize()
		{
			fixed (FieldDesc* value = &this) {
				return Functions.Native.Call<int>((void*) Imports[nameof(LoadSize)], value);
			}
		}

		[ImportCall(ImportCallOptions.Map)]
		internal void* GetCurrentStaticAddress()
		{
			fixed (FieldDesc* value = &this) {
				return Functions.Native.CallReturnPointer((void*) Imports[nameof(GetCurrentStaticAddress)], value);
			}
		}

		#endregion

		internal Pointer<MethodTable> ApproxEnclosingMethodTable {
			get {
				// m_pMTOfEnclosingClass.GetValue(PTR_HOST_MEMBER_TADDR(FieldDesc, this, m_pMTOfEnclosingClass));

				const int MT_FIELD_OFS = 0;
				
				return ClrAccess.FieldOffset((MethodTable*) EnclosingMethodTableStub.Value, MT_FIELD_OFS);
			}
		}
	}
}
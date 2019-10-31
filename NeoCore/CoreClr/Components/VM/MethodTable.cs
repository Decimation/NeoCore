using System;
using System.Runtime.InteropServices;
using NeoCore.CoreClr.Components.Support;
using NeoCore.CoreClr.Components.VM.EE;
using NeoCore.CoreClr.Meta.Base;
using NeoCore.Import;
using NeoCore.Import.Attributes;
using NeoCore.Interop;
using NeoCore.Interop.Attributes;
using NeoCore.Memory.Pointers;

// ReSharper disable UnassignedGetOnlyAutoProperty
// ReSharper disable InconsistentNaming

namespace NeoCore.CoreClr.Components.VM
{
	[ImportNamespace]
	[NativeStructure]
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct MethodTable : IClrStructure
	{
		[ImportMapField]
		private static readonly ImportMap Imports = new ImportMap();

		internal short ComponentSize { get; }

		internal GenericsFlags GenericsFlags { get; }

		internal int BaseSize { get; }

		internal OptionalSlotsFlags SlotsFlags { get; }

		internal short RawToken { get; }

		internal short NumVirtuals { get; }

		internal short NumInterfaces { get; }

		internal void* Parent { get; }

		internal void* Module { get; }

		internal void* WriteableData { get; }

		internal TypeFlags TypeFlags {
			get {
				fixed (MethodTable* ptr = &this) {
					return (TypeFlags) (*(int*) ptr);
				}
			}
		}

		#region Union 1

		/// <summary>
		///     <para>Union 1</para>
		///     <para><see cref="EEClass" /></para>
		///     <para><see cref="Canon" /></para>
		/// </summary>
		private void* Union1 { get; }

		[Obsolete]
		internal Pointer<MethodTable> Canon => (MethodTable*) Union1;

		#endregion

		#region Union 2

		/// <summary>
		///     <para>Union 2</para>
		///     <para><see cref="PerInstInfo" /></para>
		///     <para><see cref="ElementTypeHandle" /></para>
		///     <para><see cref="MultipurposeSlot1" /></para>
		/// </summary>
		private void* Union2 { get; }

		internal Pointer<byte> PerInstInfo => Union2;

		internal Pointer<MethodTable> ElementTypeHandle => Union2;

		internal Pointer<byte> MultipurposeSlot1 => Union2;

		#endregion

		#region Union 3

		/// <summary>
		///     <para>Union 3</para>
		///     <para><see cref="InterfaceMap" /></para>
		///     <para><see cref="MultipurposeSlot2" /></para>
		/// </summary>
		private void* Union3 { get; }

		internal Pointer<byte> InterfaceMap => Union3;

		internal Pointer<byte> MultipurposeSlot2 => Union3;

		#endregion

		/// <summary>
		///     Describes what the union at offset of <see cref="Union1" />
		///     contains.
		/// </summary>
		internal UnionType UnionType {
			get {
				const long UNION_MASK = 3;
				long       l          = (long) Union1;
				return (UnionType) (l & UNION_MASK);
			}
		}

		internal Pointer<EEClass> EEClass {
			[ImportCall("GetClass_NoLogging")]
			get {
				fixed (MethodTable* ptr = &this) {
					return Functions.Native.CallReturnPointer(Imports[nameof(EEClass)].ToPointer(), ptr);
				}
			}
		}

		public ClrStructureType Type => ClrStructureType.Metadata;
	}
}
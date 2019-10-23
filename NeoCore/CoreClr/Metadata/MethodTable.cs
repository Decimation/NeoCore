using System;
using System.Runtime.InteropServices;
using NeoCore.Assets;
using NeoCore.Import;
using NeoCore.Import.Attributes;
using NeoCore.Interop;
using NeoCore.Interop.Attributes;
using NeoCore.Memory;

// ReSharper disable UnassignedGetOnlyAutoProperty
// ReSharper disable InconsistentNaming

namespace NeoCore.CoreClr.Metadata
{
	
	[ImportNamespace]
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct MethodTable
	{
		static MethodTable()
		{
			ImportManager.Value.Load(typeof(MethodTable), Resources.Clr.Imports);
		}

		internal short ComponentSize { get; }

		internal GenericsFlags Generics { get; }

		internal int BaseSize { get; }

		internal SlotsFlags Slots { get; }

		internal short RawToken { get; }

		internal short NumVirtuals { get; }

		internal short NumInterfaces { get; }

		internal void* Parent { get; }

		internal void* Module { get; }

		internal void* WriteableData { get; }

		internal TypeHierarchy Hierarchy {
			get {
				fixed (MethodTable* ptr = &this) {
					return (TypeHierarchy) (*(int*) ptr);
				}
			}
		}

		#region Union 1

		/// <summary>
		///     <para>Union 1</para>
		///     <para>EEClass* <see cref="EEClass" /></para>
		///     <para>MethodTable* <see cref="Canon" /></para>
		/// </summary>
		private void* Union1 { get; }

		internal Pointer<EEClass>     EEClass => (EEClass*) Union1;
		internal Pointer<MethodTable> Canon   => (MethodTable*) Union1;

		#endregion

		#region Union 2

		/// <summary>
		///     <para>Union 2</para>
		///     <para>void* <see cref="PerInstInfo" /></para>
		///     <para>void* <see cref="ElementTypeHandle" /></para>
		///     <para>void* <see cref="MultipurposeSlot1" /></para>
		/// </summary>
		private void* Union2 { get; }

		internal Pointer<byte> PerInstInfo => Union2;

		internal Pointer<byte> ElementTypeHandle => Union2;

		internal Pointer<byte> MultipurposeSlot1 => Union2;

		#endregion

		#region Union 3

		/// <summary>
		///     <para>Union 3</para>
		///     <para>void* <see cref="InterfaceMap" /></para>
		///     <para>void* <see cref="MultipurposeSlot2" /></para>
		/// </summary>
		private void* Union3 { get; }

		internal Pointer<byte> InterfaceMap => Union3;

		internal Pointer<byte> MultipurposeSlot2 => Union3;

		#endregion

		/// <summary>
		///     Bit mask for <see cref="UnionType" />
		/// </summary>
		private const long UNION_MASK = 3;

		/// <summary>
		///     Describes what the union at offset <c>40</c> (<see cref="Union1" />)
		///     contains.
		/// </summary>
		internal UnionType UnionType {
			get {
				long l = (long) Union1;
				return (UnionType) (l & UNION_MASK);
			}
		}
	}
}
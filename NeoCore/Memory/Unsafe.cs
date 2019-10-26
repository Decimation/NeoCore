#region

using System;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using InlineIL;
using NeoCore.Assets;
using NeoCore.CoreClr;
using NeoCore.CoreClr.Meta;
using NeoCore.CoreClr.VM;
using NeoCore.Interop;
using NeoCore.Interop.Attributes;
using NeoCore.Memory.Pointers;
using NeoCore.Utilities;
using NeoCore.Utilities.Diagnostics;

#endregion

namespace NeoCore.Memory
{
	/// <summary>
	///     Provides utilities for manipulating pointers, memory, and types. This class has
	/// CompilerServices.Unsafe built in.
	///     <seealso cref="BitConverter" />
	///     <seealso cref="System.Convert" />
	///     <seealso cref="MemoryMarshal" />
	///     <seealso cref="Marshal" />
	///     <seealso cref="Span{T}" />
	///     <seealso cref="Memory{T}" />
	///     <seealso cref="Buffer" />
	///     <seealso cref="Mem" />
	///     <seealso cref="System.Runtime.CompilerServices.Unsafe" />
	/// 	<seealso cref="System.Runtime.CompilerServices.JitHelpers" />
	/// </summary>
	public static unsafe partial class Unsafe
	{
		/// <summary>
		///     <para>Returns the address of <paramref name="value" />.</para>
		/// </summary>
		/// <param name="value">Value to return the address of.</param>
		/// <returns>The address of the type in memory.</returns>
		public static Pointer<T> AddressOf<T>(ref T value)
		{
			/*var tr = __makeref(t);
			return *(IntPtr*) (&tr);*/

			return AsPointer(ref value);
		}

		public static bool TryGetAddressOfHeap<T>(T value, OffsetOptions options, out Pointer<byte> ptr)
		{
			if (Runtime.Info.IsStruct(value)) {
				ptr = null;
				return false;
			}

			ptr = AddressOfHeapInternal(value, options);
			return true;
		}

		public static bool TryGetAddressOfHeap<T>(T value, out Pointer<byte> ptr)
		{
			return TryGetAddressOfHeap(value, OffsetOptions.None, out ptr);
		}

		/// <summary>
		///     Returns the address of the data of <paramref name="value"/>. If <typeparamref name="T" /> is a value type,
		///     this will return <see cref="AddressOf{T}" />. If <typeparamref name="T" /> is a reference type,
		///     this will return the equivalent of <see cref="AddressOfHeap{T}(T, OffsetOptions)" /> with
		///     <see cref="OffsetOptions.Fields" />.
		/// </summary>
		public static Pointer<byte> AddressOfFields<T>(ref T value)
		{
			Pointer<T> addr = AddressOf(ref value);

			return Runtime.Info.IsStruct(value) ? addr.Cast() : AddressOfHeapInternal(value, OffsetOptions.Fields);
		}


		/// <summary>
		///     Returns the address of reference type <paramref name="value" />'s heap memory, offset by the specified
		///     <see cref="OffsetOptions" />.
		///     <remarks>
		///         <para>
		///             Note: This does not pin the reference in memory if it is a reference type.
		///             This may require pinning to prevent the GC from moving the object.
		///             If the GC compacts the heap, this pointer may become invalid.
		///         </para>
		///     </remarks>
		/// </summary>
		/// <param name="value">Reference type to return the heap address of</param>
		/// <param name="offset">Offset type</param>
		/// <returns>The address of <paramref name="value" /></returns>
		/// <exception cref="ArgumentOutOfRangeException">If <paramref name="offset"></paramref> is out of range.</exception>
		public static Pointer<byte> AddressOfHeap<T>(T value, OffsetOptions offset = OffsetOptions.None) where T : class
			=> AddressOfHeapInternal(value, offset);

		private static Pointer<byte> AddressOfHeapInternal<T>(T value, OffsetOptions offset)
		{
			// It is already assumed value is a class type

			//var tr = __makeref(value);
			//var heapPtr = **(IntPtr**) (&tr);

			Pointer<byte> heapPtr = AddressOf(ref value).ReadPointer();


			// NOTE:
			// Strings have their data offset by Offsets.OffsetToStringData
			// Arrays have their data offset by IntPtr.Size * 2 bytes (may be different for 32 bit)

			var offsetValue = 0;

			switch (offset) {
				case OffsetOptions.StringData:
					Guard.Assert(Runtime.Info.IsString(value));
					offsetValue = Constants.Offsets.OffsetToStringData;
					break;

				case OffsetOptions.ArrayData:
					Guard.Assert(Runtime.Info.IsArray(value));
					offsetValue = Constants.Offsets.OffsetToArrayData;
					break;

				case OffsetOptions.Fields:
					offsetValue = Constants.Offsets.OffsetToData;
					break;

				case OffsetOptions.None:
					break;

				case OffsetOptions.Header:
					offsetValue = -Constants.Offsets.OffsetToData;
					break;

				default:
					throw new ArgumentOutOfRangeException(nameof(offset), offset, null);
			}

			return heapPtr + offsetValue;
		}


		#region Sizes

		public static int SizeOf<T>(SizeOfOptions options) => SizeOf<T>(default, options);

		public static int SizeOf<T>(T value, SizeOfOptions options = SizeOfOptions.Intrinsic)
		{
			MetaType mt = typeof(T);

			if (options == SizeOfOptions.Auto) {
				// Break into the next switch branch which will go to resolved case
				options = Runtime.Info.IsStruct(value) ? SizeOfOptions.Intrinsic : SizeOfOptions.Heap;
			}

			// If a value was supplied
			if (!Runtime.Info.IsNil(value)) {
				mt = new MetaType(value.GetType());

				switch (options) {
					case SizeOfOptions.BaseFields:   return mt.InstanceFieldsSize;
					case SizeOfOptions.BaseInstance: return mt.BaseSize;
					case SizeOfOptions.Heap:         return HeapSizeOfInternal(value);
					case SizeOfOptions.Data:         return SizeOfData(value);
					case SizeOfOptions.BaseData:     return BaseSizeOfData(mt.RuntimeType);
				}
			}

			// Note: Arrays native size == 0
			// Note: Arrays have no layout

			switch (options) {
				case SizeOfOptions.Native:
					return mt.NativeSize;

				case SizeOfOptions.Managed:
					return mt.HasLayout ? mt.LayoutInfo.ManagedSize : Constants.INVALID_VALUE;

				case SizeOfOptions.Intrinsic:
					return SizeOf<T>();
				case SizeOfOptions.BaseFields:
					return mt.InstanceFieldsSize;

				case SizeOfOptions.BaseInstance:
					Guard.Assert(!Runtime.StaticInfo.IsStruct<T>());
					return mt.BaseSize;

				case SizeOfOptions.Heap:
				case SizeOfOptions.Data:
					throw new ArgumentException($"A value must be supplied to use {options}");

				case SizeOfOptions.Auto:
				case SizeOfOptions.BaseData:
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(options), options, null);
			}

			static int SizeOfData(T data)
			{
				if (Runtime.Info.IsStruct(data)) {
					return SizeOf<T>();
				}

				// Subtract the size of the ObjHeader and MethodTable*
				return HeapSizeOfInternal(data) - Constants.Sizes.ObjectBaseSize;
			}

			static int BaseSizeOfData(Type type)
			{
				var mtx = (MetaType) type;

				if (mtx.IsStruct) {
					return (int) Functions.Reflection.CallGeneric(typeof(Unsafe).GetMethod(nameof(SizeOf)),
					                                              type, null);
				}

				// Subtract the size of the ObjHeader and MethodTable*
				return mtx.InstanceFieldsSize;
			}


			return Constants.INVALID_VALUE;
		}


		#region HeapSize

		/// <summary>
		///     <para>Calculates the complete size of a reference type in heap memory.</para>
		///     <para>This is the most accurate size calculation.</para>
		///     <para>
		///         This follows the size formula of: (<see cref="MethodTable.BaseSize" />) + (length) *
		///         (<see cref="MethodTable.ComponentSize" />)
		///     </para>
		///     <para>where:</para>
		///     <list type="bullet">
		///         <item>
		///             <description>
		///                 <see cref="MethodTable.BaseSize" /> = The base instance size of a type
		///                 (<c>24</c> (x64) or <c>12</c> (x86) by default) (<see cref="Constants.Sizes.MinObjectSize" />)
		///             </description>
		///         </item>
		///         <item>
		///             <description>length	= array or string length; <c>1</c> otherwise</description>
		///         </item>
		///         <item>
		///             <description><see cref="MethodTable.ComponentSize" /> = element size, if available; <c>0</c> otherwise</description>
		///         </item>
		///     </list>
		/// </summary>
		/// <remarks>
		///     <para>Source: /src/vm/object.inl: 45</para>
		///     <para>Equals the Son Of Strike "!do" command.</para>
		///     <para>
		///         Equals <see cref="SizeOf{T}(T,SizeOfOptions)" /> with <see cref="SizeOfOptions.BaseInstance" /> for objects
		///         that aren't arrays or strings.
		///     </para>
		///     <para>Note: This also includes padding and overhead (<see cref="ObjHeader" /> and <see cref="MethodTable" /> ptr.)</para>
		/// </remarks>
		/// <returns>The size of the type in heap memory, in bytes</returns>
		public static int HeapSizeOf<T>(T value) where T : class
			=> HeapSizeOfInternal(value);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static int HeapSizeOfInternal<T>(T value)
		{
			// Sanity check
			Guard.Assert(!Runtime.Info.IsStruct(value));

			if (Runtime.Info.IsNil(value)) {
				return Constants.INVALID_VALUE;
			}

			// By manually reading the MethodTable*, we can calculate the size correctly if the reference
			// is boxed or cloaked
			var methodTable = Runtime.ReadTypeHandle(value);

			// Value of GetSizeField()
			int length = 0;

			/**
			 * Type			x86 size				x64 size
			 *
			 * object		12						24
			 * object[]		16 + length * 4			32 + length * 8
			 * int[]		12 + length * 4			28 + length * 4
			 * byte[]		12 + length				24 + length
			 * string		14 + length * 2			26 + length * 2
			 */

			// From object.h line 65:

			/* 	  The size of the object in the heap must be able to be computed
			 *    very, very quickly for GC purposes.   Restrictions on the layout
			 *    of the object guarantee this is possible.
			 *
			 *    Any object that inherits from Object must be able to
			 *    compute its complete size by using the first 4 bytes of
			 *    the object following the Object part and constants
			 *    reachable from the MethodTable...
			 *
			 *    The formula used for this calculation is:
			 *        MT->GetBaseSize() + ((OBJECTTYPEREF->GetSizeField() * MT->GetComponentSize())
			 *
			 *    So for Object, since this is of fixed size, the ComponentSize is 0, which makes the right side
			 *    of the equation above equal to 0 no matter what the value of GetSizeField(), so the size is just the base size.
			 *
			 */

			if (Runtime.Info.IsArray(value)) {
				var arr = value as Array;

				// ReSharper disable once PossibleNullReferenceException
				// We already know it's not null because the type is an array.
				length = arr.Length;

				// Sanity check
				Guard.Assert(!Runtime.Info.IsString(value));
			}
			else if (Runtime.Info.IsString(value)) {
				string str = value as string;

				// Sanity check
				Guard.Assert(!Runtime.Info.IsArray(value));
				Guard.AssertNotNull(str, nameof(str));

				length = str.Length;
			}

			return methodTable.BaseSize + length * methodTable.ComponentSize;
		}

		#endregion

		#endregion
	}
}
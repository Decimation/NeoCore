using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Memkit;
using Memkit.Pointers;
using Memkit.Utilities;
using NeoCore.CoreClr;
using NeoCore.CoreClr.Meta;
using NeoCore.CoreClr.VM;
using NeoCore.CoreClr.VM.EE;
using NeoCore.Utilities.Diagnostics;
using NeoCore.Win32;

namespace NeoCore.Memory
{
	/// <summary>
	///     Provides utilities for manipulating pointers, memory, and types. This class has
	///     <seealso cref="System.Runtime.CompilerServices.Unsafe" /> built in.
	///     Also see JitHelpers from CompilerServices.
	///     <seealso cref="BitConverter" />
	///     <seealso cref="System.Convert" />
	///     <seealso cref="MemoryMarshal" />
	///     <seealso cref="Marshal" />
	///     <seealso cref="Span{T}" />
	///     <seealso cref="Memory{T}" />
	///     <seealso cref="Buffer" />
	///     <seealso cref="Mem" />
	///     <seealso cref="System.Runtime.CompilerServices.Unsafe" />
	///     <seealso cref="System.Runtime.CompilerServices" />
	/// </summary>
	public class Neomem
	{
		public static void Destroy<T>(ref T value)
		{
			if (!Runtime.Inspection.IsStruct(value)) {
				int           size = SizeOf(value, SizeOfOptions.Data);
				Pointer<byte> ptr  = Mem.AddressOfFields(ref value);
				ptr.Cast().Clear(size);
			}

			value = default;
		}
		
		//    Calculates the complete size of a reference type in heap memory.
		//    This is the most accurate size calculation.
		//    This follows the size formula of: (<see cref="MethodTable.BaseSize" />) + (length) *
		//    (<see cref="MethodTable.ComponentSize" />)
		//    
		//    where:
		//    
		//             - <see cref="MethodTable.BaseSize" /> = The base instance size of a type
		//                (<c>24</c> (x64) or <c>12</c> (x86) by default) (<see cref="Assets.MinObjectSize" />)
		//
		//             - length	= array or string length; <c>1</c> otherwise
		//             - <see cref="MethodTable.ComponentSize" /> = element size, if available; <c>0</c> otherwise
		//        
		//    
		//
		//
		//    Source: /src/vm/object.inl: 45
		//    Equals the Son Of Strike "!do" command.
		//    
		//        Equals <see cref="SizeOf{T}(T,SizeOfOptions)" /> with <see cref="SizeOfOptions.BaseInstance" /> for objects
		//        that aren't arrays or strings.
		//    
		//    Note: This also includes padding and overhead (<see cref="ObjHeader" /> and <see cref="MethodTable" /> ptr.)
		//
		// Returns the size of the type in heap memory, in bytes

		public static int HeapSizeOf<T>(T value) where T : class
			=> HeapSizeOfInternal(value);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static int HeapSizeOfInternal<T>(T value)
		{
			// Sanity check
			Guard.Assert(!Runtime.Inspection.IsStruct(value));

			if (Runtime.Info.IsNil(value)) {
				return Assets.INVALID_VALUE;
			}

			// By manually reading the MethodTable*, we can calculate the size correctly if the reference
			// is boxed or cloaked
			var methodTable = Runtime.ReadTypeHandle(value);

			// Value of GetSizeField()
			int length = 0;

			// Type			x86 size				x64 size
			// 
			// object		12						24
			// object[]		16 + length * 4			32 + length * 8
			// int[]		12 + length * 4			28 + length * 4
			// byte[]		12 + length				24 + length
			// string		14 + length * 2			26 + length * 2

			// From object.h line 65:

			// The size of the object in the heap must be able to be computed
			// very, very quickly for GC purposes.   Restrictions on the layout
			// of the object guarantee this is possible.
			// 
			// Any object that inherits from Object must be able to
			// compute its complete size by using the first 4 bytes of
			// the object following the Object part and constants
			// reachable from the MethodTable...
			// 
			// The formula used for this calculation is:
			//     MT->GetBaseSize() + ((OBJECTTYPEREF->GetSizeField() * MT->GetComponentSize())
			// 
			// So for Object, since this is of fixed size, the ComponentSize is 0, which makes the right side
			// of the equation above equal to 0 no matter what the value of GetSizeField(), so the size is just the base size.

			if (Runtime.Inspection.IsArray(value)) {
				var arr = value as Array;

				// ReSharper disable once PossibleNullReferenceException
				// We already know it's not null because the type is an array.
				length = arr.Length;

				// Sanity check
				Guard.Assert(!Runtime.Inspection.IsString(value));
			}
			else if (Runtime.Inspection.IsString(value)) {
				string str = value as string;

				// Sanity check
				Guard.Assert(!Runtime.Inspection.IsArray(value));
				Guard.AssertNotNull(str, nameof(str));

				length = str.Length;
			}

			return methodTable.BaseSize + length * methodTable.ComponentSize;
		}


		public static int SizeOf<T>(SizeOfOptions options = SizeOfOptions.Auto) => SizeOf<T>(default, options);

		public static int SizeOf<T>(T value, SizeOfOptions options = SizeOfOptions.Intrinsic)
		{
			MetaType mt = typeof(T);

			if (options == SizeOfOptions.Auto) {
				// Break into the next switch branch which will go to resolved case
				options = Runtime.Inspection.IsStruct(value) ? SizeOfOptions.Intrinsic : SizeOfOptions.Heap;
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
					return mt.HasLayout ? mt.LayoutInfo.ManagedSize : Assets.INVALID_VALUE;

				case SizeOfOptions.Intrinsic:
					return SizeOf<T>();
				case SizeOfOptions.BaseFields:
					return mt.InstanceFieldsSize;

				case SizeOfOptions.BaseInstance:
					Guard.Assert(!Runtime.Inspection.IsCompileStruct<T>());
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
				if (Runtime.Inspection.IsStruct(data)) {
					return SizeOf<T>();
				}

				// Subtract the size of the ObjHeader and MethodTable*
				return HeapSizeOfInternal(data) - ClrAssets.ObjectBaseSize;
			}

			static int BaseSizeOfData(Type type)
			{
				var mtx = (MetaType) type;

				if (mtx.IsStruct) {
					return (int) Functions.Reflection.CallGeneric(typeof(Mem).GetMethod(nameof(SizeOf)),
					                                              type, null);
				}

				// Subtract the size of the ObjHeader and MethodTable*
				return mtx.InstanceFieldsSize;
			}


			return Assets.INVALID_VALUE;
		}
	}
	
	

	/// <summary>
	/// Specifies how sizes are calculated
	/// </summary>
	public enum SizeOfOptions
	{
		/// <summary>
		///     <para>Returns the native (<see cref="Marshal" />) size of a type.</para>
		/// </summary>
		/// <remarks>
		/// <para>Only a type parameter is needed</para>
		///     <para> Returned from <see cref="EEClass.NativeSize" /> </para>
		///     <para> Equals <see cref="Marshal.SizeOf(Type)" /></para>
		///     <para> Equals <see cref="StructLayoutAttribute.Size" /> when type isn't zero-sized.</para>
		/// </remarks>
		/// <returns>The native size if the type has a native representation; <see cref="Assets.INVALID_VALUE" /> otherwise</returns>
		Native,

		/// <summary>
		///     Returns the managed size of an object.
		/// </summary>
		/// <remarks>
		/// <para>Only a type parameter is needed</para>
		///     <para>Returned from <see cref="EEClassLayoutInfo.ManagedSize" /></para>
		/// </remarks>
		/// <returns>
		///     Managed size if the type has an <see cref="EEClassLayoutInfo" />; <see cref="Assets.INVALID_VALUE" />
		///     otherwise
		/// </returns>
		Managed,


		/// <summary>
		///     <para>Returns the normal size of a type in memory.</para>
		///     <para>Call to <see cref="Mem.SizeOf{T}()" /></para>
		/// </summary>
		/// <remarks>
		/// <para>Only a type parameter is needed</para>
		/// </remarks>
		/// <returns><see cref="IntPtr.Size" /> for reference types, size for value types</returns>
		Intrinsic,

		/// <summary>
		///     <para>Returns the base size of the fields (data) in the heap.</para>
		///     <para>This follows the formula of:</para>
		///     <para><see cref="MetaType.BaseSize" /> - <see cref="MetaType.BaseSizePadding" /></para>
		/// <para>
		///         If a value is supplied, this manually reads the <c>MethodTable*</c>, making
		///         this work for boxed values.
		///     </para>
		/// <code>
		/// object o = 123;
		/// Unsafe.BaseFieldsSize(ref o);  // == 4 (boxed int, base fields size of int (sizeof(int)))
		/// Unsafe.BaseFieldsSize&lt;object&gt;; // == 0 (base fields size of object)
		/// </code>
		///     <remarks>
		/// <para>Only a type parameter is needed, or a value can be supplied</para>
		///         <para>Supply a value if the value may be boxed.</para>
		///         <para>Returned from <see cref="MetaType.InstanceFieldsSize" /></para>
		///         <para>This includes field padding.</para>
		///     </remarks>
		/// </summary>
		/// <returns><see cref="Assets.MinObjectSize" /> if type is an array, fields size otherwise</returns>
		BaseFields,

		/// <summary>
		///     <para>Returns the base instance size according to the TypeHandle (<c>MethodTable</c>).</para>
		///     <para>This is the minimum heap size of a type.</para>
		///     <para>By default, this equals <see cref="Assets.MinObjectSize" /> (<c>24</c> (x64) or <c>12</c> (x84)).</para>
		/// </summary>
		/// <remarks>
		/// <para>Only a type parameter is needed, or a value can be supplied</para>
		///     <para>Returned from <see cref="MetaType.BaseSize" /></para>
		/// </remarks>
		/// <returns>
		///     <see cref="MetaType.BaseSize" />
		/// </returns>
		BaseInstance,

		/// <summary>
		///     <para>Calculates the complete size of a reference type in heap memory.</para>
		///     <para>This is the most accurate size calculation.</para>
		///     <para>
		///         This follows the size formula of: (<see cref="MetaType.BaseSize" />) + (length) *
		///         (<see cref="MetaType.ComponentSize" />)
		///     </para>
		///     <para>where:</para>
		///     <list type="bullet">
		///         <item>
		///             <description>
		///                 <see cref="MetaType.BaseSize" /> = The base instance size of a type
		///                 (<c>24</c> (x64) or <c>12</c> (x86) by default) (<see cref="Assets.MinObjectSize" />)
		///             </description>
		///         </item>
		///         <item>
		///             <description>length	= array or string length; <c>1</c> otherwise</description>
		///         </item>
		///         <item>
		///             <description><see cref="MetaType.ComponentSize" /> = element size, if available; <c>0</c> otherwise</description>
		///         </item>
		///     </list>
		/// </summary>
		/// <remarks>
		/// <para>A value must be supplied.</para>
		///     <para>Source: /src/vm/object.inl: 45</para>
		///     <para>Equals the Son Of Strike "!do" command.</para>
		///     <para>Equals <see cref="BaseInstance" /> for objects that aren't arrays or strings.</para>
		///     <para>Note: This also includes padding and overhead (<see cref="ObjHeader" /> and <see cref="MethodTable" /> ptr.)</para>
		/// </remarks>
		/// <returns>The size of the type in heap memory, in bytes</returns>
		Heap,

		/// <summary>
		///     Calculates the complete size of the value's data. If the type parameter is
		///     a value type, this is equal to option <see cref="Intrinsic"/>. If the type parameter is a
		///     reference type, this is equal to <see cref="Heap"/>.
		/// </summary>
		Auto,

		/// <summary>
		/// Requires a value.
		/// Returns the size of the data in the value. If the type is a reference type,
		/// this returns the size of the value not occupied by the <see cref="MethodTable" /> pointer and the
		/// <see cref="ObjHeader" />.
		/// If the type is a value type, this returns <see cref="Mem.SizeOf{T}()" />.
		/// </summary>
		Data,

		/// <summary>
		/// Does not require a value.
		/// Returns the base size of the data in the type specified by the value. If the value is a
		/// reference type,
		/// this returns the size of data not occupied by the <see cref="MethodTable" /> pointer, <see cref="ObjHeader" />,
		/// padding, and overhead.
		/// If the value is a value type, this returns <see cref="Mem.SizeOf{T}()" />.
		/// 
		/// </summary>
		BaseData,
	}
}
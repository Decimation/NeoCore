using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using InlineIL;
using NeoCore.CoreClr;
using NeoCore.CoreClr.Meta;
using NeoCore.CoreClr.VM;
using NeoCore.CoreClr.VM.EE;
using NeoCore.Interop;
using NeoCore.Interop.Attributes;
using NeoCore.Interop.Structures;
using NeoCore.Memory.Pointers;
using NeoCore.Utilities;
using NeoCore.Utilities.Extensions;
using System.Diagnostics;

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
	public static unsafe class Mem
	{
		public static Region StackRegion {
			get {
				var info = new MemoryBasicInfo();
				var ptr  = new IntPtr(&info);
				Native.Kernel.VirtualQuery(ptr, ref info, Marshal.SizeOf<MemoryBasicInfo>());

				// todo: verify
				long size = (info.BaseAddress.ToInt64() - info.AllocationBase.ToInt64()) + info.RegionSize.ToInt64();

				return new Region(info.AllocationBase, size);
			}
		}

		public static bool IsAddressInRange(Pointer<byte> p, Region r)
		{
			return IsAddressInRange(p, r.Low, r.High);
		}

		/// <param name="p">Operand</param>
		/// <param name="lo">Start address (inclusive)</param>
		/// <param name="hi">End address (inclusive)</param>
		public static bool IsAddressInRange(Pointer<byte> p, Pointer<byte> lo, Pointer<byte> hi)
		{
			// [lo, hi]

			// if ((ptrStack < stackBase) && (ptrStack > (stackBase - stackSize)))
			// (p >= regionStart && p < regionStart + regionSize) ;
			// return target >= start && target < end;
			// return m_CacheStackLimit < addr && addr <= m_CacheStackBase;
			// if (!((object < g_gc_highest_address) && (object >= g_gc_lowest_address)))
			// return max.ToInt64() < p.ToInt64() && p.ToInt64() <= min.ToInt64();

			return p <= hi && p >= lo;
		}

		public static bool IsAddressInRange(Pointer<byte> p, Pointer<byte> lo, long size)
		{
			// if ((ptrStack < stackBase) && (ptrStack > (stackBase - stackSize)))
			// (p >= regionStart && p < regionStart + regionSize) ;
			// return target >= start && target < end;
			// return m_CacheStackLimit < addr && addr <= m_CacheStackBase;
			// if (!((object < g_gc_highest_address) && (object >= g_gc_lowest_address)))
			// return max.ToInt64() < p.ToInt64() && p.ToInt64() <= min.ToInt64();


			return (p >= lo && p < lo + size);
		}

		/// <summary>
		/// Calculates the total byte size of <paramref name="elemCnt"/> elements with
		/// the size of <paramref name="elemSize"/>.
		/// </summary>
		/// <param name="elemSize">Byte size of one element</param>
		/// <param name="elemCnt">Number of elements</param>
		/// <returns>Total byte size of all elements</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int FlatSize(int elemSize, int elemCnt)
		{
			// (void*) (((long) m_value) + byteOffset)
			// (void*) (((long) m_value) + (elemOffset * ElementSize))
			return elemCnt * elemSize;
		}

		/// <summary>
		/// Reads a simple structure using stack allocation.
		/// </summary>
		public static T ReadStructure<T>(byte[] bytes) where T : struct
		{
			// Pin the managed memory while, copy it out the data, then unpin it

			byte* handle = stackalloc byte[bytes.Length];
			Marshal.Copy(bytes, 0, (IntPtr) handle, bytes.Length);

			var value = (T) Marshal.PtrToStructure((IntPtr) handle, typeof(T));

			return value;
		}


		/*[NativeFunction]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T ReadFast<T>(void* source, int elemOfs)
		{
			IL.Emit.Ldarg(nameof(elemOfs));
			IL.Emit.Sizeof(typeof(T));
			IL.Emit.Mul();
			IL.Emit.Ldarg(nameof(source));
			IL.Emit.Add();
			IL.Emit.Ldobj(typeof(T));
			return IL.Return<T>();
		}*/

		public static int Val32(int i)
		{
			// todo
			return i;
		}

		public static short Val16(short i)
		{
			// todo
			return i;
		}

		public static string ReadString(sbyte* first, int len)
		{
			if (first == null || len <= 0) {
				return null;
			}

			return new string(first, 0, len);
		}

		public static void Destroy<T>(ref T value)
		{
			if (!Runtime.Inspection.IsStruct(value)) {
				int           size = SizeOf(value, SizeOfOptions.Data);
				Pointer<byte> ptr  = AddressOfFields(ref value);
				ptr.Cast().Clear(size);
			}

			value = default;
		}

		/// <summary>
		///     Used for unsafe pinning of arbitrary objects.
		/// </summary>
		public static PinHelper GetPinHelper(object value) => Unsafe.As<PinHelper>(value);

		/// <summary>
		///     <para>Returns the address of <paramref name="value" />.</para>
		/// </summary>
		/// <param name="value">Value to return the address of.</param>
		/// <returns>The address of the type in memory.</returns>
		public static Pointer<T> AddressOf<T>(ref T value)
		{
			/*var tr = __makeref(t);
			return *(IntPtr*) (&tr);*/

			return Unsafe.AsPointer(ref value);
		}

		public static bool TryGetAddressOfHeap<T>(T value, OffsetOptions options, out Pointer<byte> ptr)
		{
			if (Runtime.Inspection.IsStruct(value)) {
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
		///     Returns the address of the data of <paramref name="value" />. If <typeparamref name="T" /> is a value type,
		///     this will return <see cref="AddressOf{T}" />. If <typeparamref name="T" /> is a reference type,
		///     this will return the equivalent of <see cref="AddressOfHeap{T}(T, OffsetOptions)" /> with
		///     <see cref="OffsetOptions.Fields" />.
		/// </summary>
		public static Pointer<byte> AddressOfFields<T>(ref T value)
		{
			Pointer<T> addr = AddressOf(ref value);

			return Runtime.Inspection.IsStruct(value)
				? addr.Cast()
				: AddressOfHeapInternal(value, OffsetOptions.Fields);
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

			int offsetValue = 0;

			switch (offset) {
				case OffsetOptions.StringData:
					Guard.Assert(Runtime.Inspection.IsString(value));
					offsetValue = Assets.OffsetToStringData;
					break;

				case OffsetOptions.ArrayData:
					Guard.Assert(Runtime.Inspection.IsArray(value));
					offsetValue = Assets.OffsetToArrayData;
					break;

				case OffsetOptions.Fields:
					offsetValue = Assets.OffsetToData;
					break;

				case OffsetOptions.None:
					break;

				case OffsetOptions.Header:
					offsetValue = -Assets.OffsetToData;
					break;

				default:
					throw new ArgumentOutOfRangeException(nameof(offset), offset, null);
			}

			return heapPtr + offsetValue;
		}


		#region Fields

		/// <summary>
		/// Size of a pointer. Equals <see cref="IntPtr.Size"/>.
		/// </summary>
		public static readonly int Size = IntPtr.Size;

		/// <summary>
		/// Determines whether this process is 64-bit.
		/// </summary>
		public static readonly bool Is64Bit = Size == sizeof(long) && Environment.Is64BitProcess;

		public static readonly bool IsBigEndian = !BitConverter.IsLittleEndian;

		/// <summary>
		/// Represents a <c>null</c> <see cref="Pointer{T}"/>. Equivalent to <see cref="IntPtr.Zero"/>.
		/// </summary>
		public static readonly Pointer<byte> Nullptr = null;

		#endregion


		#region Sizes

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
				return HeapSizeOfInternal(data) - Assets.ObjectBaseSize;
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
		///                 (<c>24</c> (x64) or <c>12</c> (x86) by default) (<see cref="Assets.MinObjectSize" />)
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

		#endregion

		#endregion

		#region Read / Write

		public static T ReadCurrentProcessMemory<T>(Pointer<byte> ptrBase) =>
			ReadProcessMemory<T>(Process.GetCurrentProcess(), ptrBase);

		public static T ReadProcessMemory<T>(Process proc, Pointer<byte> ptrBase)
		{
			T          t    = default;
			int        size = Mem.SizeOf<T>();
			Pointer<T> ptr  = Mem.AddressOf(ref t);

			ReadProcessMemory(proc, ptrBase.Address, ptr.Address, size);

			return t;
		}

		public static void WriteCurrentProcessMemory<T>(Pointer<byte> ptrBase, T value) =>
			WriteProcessMemory(Process.GetCurrentProcess(), ptrBase, value);

		public static void WriteProcessMemory<T>(Process proc, Pointer<byte> ptrBase, T value)
		{
			int        dwSize = Mem.SizeOf<T>();
			Pointer<T> ptr    = Mem.AddressOf(ref value);

			WriteProcessMemory(proc, ptrBase.Address, ptr.Address, dwSize);
		}

		#endregion

		#region Read / write raw bytes

		#region Read raw bytes

		public static void ReadProcessMemory(Process       proc,      Pointer<byte> ptrBase,
		                                     Pointer<byte> ptrBuffer, int           cb)
		{
			var hProc = Native.Kernel.OpenProcess(proc);


			// Read the memory
			bool ok = (Native.Kernel.ReadProcMemoryInternal(hProc, ptrBase.Address,
			                                                ptrBuffer.Address, cb,
			                                                out int numberOfBytesRead));

			Guard.AssertWin32(numberOfBytesRead == cb && ok);

			// Close the handle
			Native.Kernel.CloseHandle(hProc);
		}

		public static byte[] ReadProcessMemory(Process proc, Pointer<byte> ptrBase, int cb)
		{
			var mem = new byte[cb];

			fixed (byte* p = mem) {
				ReadProcessMemory(proc, ptrBase, (IntPtr) p, cb);
			}

			return mem;
		}


		#region Current process

		public static byte[] ReadCurrentProcessMemory(Pointer<byte> ptrBase, int cb) =>
			ReadProcessMemory(Process.GetCurrentProcess(), ptrBase, cb);

		public static void ReadCurrentProcessMemory(Pointer<byte> ptrBase, Pointer<byte> ptrBuffer, int cb) =>
			ReadProcessMemory(Process.GetCurrentProcess(), ptrBase, ptrBuffer, cb);

		#endregion

		#endregion

		#region Write raw bytes

		#region Current process

		public static void WriteCurrentProcessMemory(Pointer<byte> ptrBase, byte[] value) =>
			WriteProcessMemory(Process.GetCurrentProcess(), ptrBase, value);

		public static void WriteCurrentProcessMemory(Pointer<byte> ptrBase, Pointer<byte> ptrBuffer,
		                                             int           dwSize) =>
			WriteProcessMemory(Process.GetCurrentProcess(), ptrBase, ptrBuffer, dwSize);

		#endregion

		public static void WriteProcessMemory(Process proc, Pointer<byte> ptrBase, Pointer<byte> ptrBuffer,
		                                      int     dwSize)
		{
			var hProc = Native.Kernel.OpenProcess(proc);

			// Write the memory
			bool ok = (Native.Kernel.WriteProcMemoryInternal(hProc, ptrBase.Address, ptrBuffer.Address,
			                                                 dwSize, out int numberOfBytesWritten));


			Guard.AssertWin32(numberOfBytesWritten == dwSize && ok);


			// Close the handle
			Native.Kernel.CloseHandle(hProc);
		}

		public static void WriteProcessMemory(Process proc, Pointer<byte> ptrBase, byte[] value)
		{
			fixed (byte* rg = value) {
				WriteProcessMemory(proc, ptrBase, (IntPtr) rg, value.Length);
			}
		}

		#endregion

		#endregion
	}

	/// <summary>
	///     Offset options for <see cref="Mem.AddressOfHeap{T}(T,OffsetOptions)" />
	/// </summary>
	public enum OffsetOptions
	{
		/// <summary>
		///     Return the pointer offset by <c>-</c><see cref="Size" />,
		///     so it points to the object's <see cref="ObjHeader" />.
		/// </summary>
		Header,

		/// <summary>
		///     If the type is a <see cref="string" />, return the
		///     pointer offset by <see cref="Assets.OffsetToStringData" /> so it
		///     points to the string's characters.
		///     <remarks>
		///         Note: Equal to <see cref="GCHandle.AddrOfPinnedObject" /> and <c>fixed</c>.
		///     </remarks>
		/// </summary>
		StringData,

		/// <summary>
		///     If the type is an array, return
		///     the pointer offset by <see cref="Assets.OffsetToArrayData" /> so it points
		///     to the array's elements.
		///     <remarks>
		///         Note: Equal to <see cref="GCHandle.AddrOfPinnedObject" /> and <c>fixed</c>
		///     </remarks>
		/// </summary>
		ArrayData,

		/// <summary>
		///     If the type is a reference type, return
		///     the pointer offset by <see cref="Size" /> so it points
		///     to the object's fields.
		/// </summary>
		Fields,

		/// <summary>
		///     Don't offset the heap pointer at all, so it
		///     points to the <see cref="TypeHandle"/>
		/// </summary>
		None
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

	/// <summary>
	///     <para>Helper class to assist with unsafe pinning of arbitrary objects. The typical usage pattern is:</para>
	///     <code>
	///  fixed (byte* pData = &amp;GetPinHelper(value).Data)
	///  {
	///  }
	///  </code>
	///     <remarks>
	///         <para><c>pData</c> is what <c>Object::GetData()</c> returns in VM.</para>
	///         <para><c>pData</c> is also equal to offsetting the pointer by <see cref="OffsetOptions.Fields" />. </para>
	///         <para>From <see cref="SOURCE" />. </para>
	///     </remarks>
	/// </summary>
	[UsedImplicitly]
	public sealed class PinHelper
	{
		private const string SOURCE = "System.Runtime.CompilerServices.JitHelpers";

		/// <summary>
		///     Represents the first field in an object, such as <see cref="OffsetOptions.Fields" />.
		/// </summary>
		public byte Data;

		private PinHelper() { }
	}
}
#region

using System;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using InlineIL;
using NeoCore.Assets;
using NeoCore.CoreClr;
using NeoCore.Interop.Attributes;
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
	public static unsafe class Unsafe
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
		

		#region Unsafe

		// https://github.com/ltrzesniewski/InlineIL.Fody/blob/master/src/InlineIL.Examples/Unsafe.cs
		// https://github.com/dotnet/corefx/blob/master/src/System.Runtime.CompilerServices.Unsafe/src/System.Runtime.CompilerServices.Unsafe.il


		[NativeFunction]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T Read<T>(void* source)
		{
			IL.Emit.Ldarg(nameof(source));
			IL.Emit.Ldobj(typeof(T));
			return IL.Return<T>();
		}

		[NativeFunction]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T ReadUnaligned<T>(void* source)
		{
			IL.Emit.Ldarg(nameof(source));
			IL.Emit.Unaligned(1);
			IL.Emit.Ldobj(typeof(T));
			return IL.Return<T>();
		}

		[NativeFunction]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T ReadUnaligned<T>(ref byte source)
		{
			IL.Emit.Ldarg(nameof(source));
			IL.Emit.Unaligned(1);
			IL.Emit.Ldobj(typeof(T));
			return IL.Return<T>();
		}

		[NativeFunction]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Write<T>(void* destination, T value)
		{
			IL.Emit.Ldarg(nameof(destination));
			IL.Emit.Ldarg(nameof(value));
			IL.Emit.Stobj(typeof(T));
		}


		[NativeFunction]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void WriteUnaligned<T>(void* destination, T value)
		{
			IL.Emit.Ldarg(nameof(destination));
			IL.Emit.Ldarg(nameof(value));
			IL.Emit.Unaligned(1);
			IL.Emit.Stobj(typeof(T));
		}


		[NativeFunction]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void WriteUnaligned<T>(ref byte destination, T value)
		{
			IL.Emit.Ldarg(nameof(destination));
			IL.Emit.Ldarg(nameof(value));
			IL.Emit.Unaligned(1);
			IL.Emit.Stobj(typeof(T));
		}


		[NativeFunction]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Copy<T>(void* destination, ref T source)
		{
			IL.Emit.Ldarg(nameof(destination));
			IL.Emit.Ldarg(nameof(source));
			IL.Emit.Ldobj(typeof(T));
			IL.Emit.Stobj(typeof(T));
		}


		[NativeFunction]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Copy<T>(ref T destination, void* source)
		{
			IL.Emit.Ldarg(nameof(destination));
			IL.Emit.Ldarg(nameof(source));
			IL.Emit.Ldobj(typeof(T));
			IL.Emit.Stobj(typeof(T));
		}


		[NativeFunction]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void* AsPointer<T>(ref T value)
		{
			IL.Emit.Ldarg(nameof(value));
			IL.Emit.Conv_U();
			return IL.ReturnPointer();
		}


		/// <summary>
		///     <para>Returns the size of a type in memory.</para>
		/// </summary>
		/// <returns><see cref="Size" /> for reference types, size for value types</returns>
		[NativeFunction]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int SizeOf<T>()
		{
			IL.Emit.Sizeof(typeof(T));
			return IL.Return<int>();
		}


		[NativeFunction]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void CopyBlock(void* destination, void* source, uint byteCount)
		{
			IL.Emit.Ldarg(nameof(destination));
			IL.Emit.Ldarg(nameof(source));
			IL.Emit.Ldarg(nameof(byteCount));
			IL.Emit.Cpblk();
		}


		[NativeFunction]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void CopyBlock(ref byte destination, ref byte source, uint byteCount)
		{
			IL.Emit.Ldarg(nameof(destination));
			IL.Emit.Ldarg(nameof(source));
			IL.Emit.Ldarg(nameof(byteCount));
			IL.Emit.Cpblk();
		}


		[NativeFunction]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void CopyBlockUnaligned(void* destination, void* source, uint byteCount)
		{
			IL.Emit.Ldarg(nameof(destination));
			IL.Emit.Ldarg(nameof(source));
			IL.Emit.Ldarg(nameof(byteCount));
			IL.Emit.Unaligned(1);
			IL.Emit.Cpblk();
		}


		[NativeFunction]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void CopyBlockUnaligned(ref byte destination, ref byte source, uint byteCount)
		{
			IL.Emit.Ldarg(nameof(destination));
			IL.Emit.Ldarg(nameof(source));
			IL.Emit.Ldarg(nameof(byteCount));
			IL.Emit.Unaligned(1);
			IL.Emit.Cpblk();
		}


		[NativeFunction]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void InitBlock(void* startAddress, byte value, uint byteCount)
		{
			IL.Emit.Ldarg(nameof(startAddress));
			IL.Emit.Ldarg(nameof(value));
			IL.Emit.Ldarg(nameof(byteCount));
			IL.Emit.Initblk();
		}


		[NativeFunction]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void InitBlock(ref byte startAddress, byte value, uint byteCount)
		{
			IL.Emit.Ldarg(nameof(startAddress));
			IL.Emit.Ldarg(nameof(value));
			IL.Emit.Ldarg(nameof(byteCount));
			IL.Emit.Initblk();
		}


		[NativeFunction]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void InitBlockUnaligned(void* startAddress, byte value, uint byteCount)
		{
			IL.Emit.Ldarg(nameof(startAddress));
			IL.Emit.Ldarg(nameof(value));
			IL.Emit.Ldarg(nameof(byteCount));
			IL.Emit.Unaligned(1);
			IL.Emit.Initblk();
		}


		[NativeFunction]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void InitBlockUnaligned(ref byte startAddress, byte value, uint byteCount)
		{
			IL.Emit.Ldarg(nameof(startAddress));
			IL.Emit.Ldarg(nameof(value));
			IL.Emit.Ldarg(nameof(byteCount));
			IL.Emit.Unaligned(1);
			IL.Emit.Initblk();
		}


		[NativeFunction]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T As<T>(object o) where T : class
		{
			IL.Emit.Ldarg(nameof(o));
			return IL.Return<T>();
		}


		[NativeFunction]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref T AsRef<T>(void* source)
		{
			// For .NET Core the roundtrip via a local is no longer needed (update the constant as needed)
#if NETCOREAPP
			IL.Push(source);
			return ref IL.ReturnRef<T>();
#else
			// Roundtrip via a local to avoid type mismatch on return that the JIT inliner chokes on.
			IL.DeclareLocals(
				false,
				new LocalVar("local", typeof(int).MakeByRefType())
			);

			IL.Push(source);
			IL.Emit.Stloc("local");
			IL.Emit.Ldloc("local");
			return ref IL.ReturnRef<T>();
#endif
		}


		[NativeFunction]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref T AsRef<T>(in T source)
		{
			IL.Emit.Ldarg(nameof(source));
			return ref IL.ReturnRef<T>();
		}


		[NativeFunction]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref TTo As<TFrom, TTo>(ref TFrom source)
		{
			IL.Emit.Ldarg(nameof(source));
			return ref IL.ReturnRef<TTo>();
		}


		[NativeFunction]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref T Unbox<T>(object box) where T : struct
		{
			IL.Push(box);
			IL.Emit.Unbox(typeof(T));
			return ref IL.ReturnRef<T>();
		}


		[NativeFunction]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref T Add<T>(ref T source, int elementOffset)
		{
			IL.Emit.Ldarg(nameof(source));
			IL.Emit.Ldarg(nameof(elementOffset));
			IL.Emit.Sizeof(typeof(T));
			IL.Emit.Conv_I();
			IL.Emit.Mul();
			IL.Emit.Add();
			return ref IL.ReturnRef<T>();
		}


		[NativeFunction]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void* Add<T>(void* source, int elementOffset)
		{
			IL.Emit.Ldarg(nameof(source));
			IL.Emit.Ldarg(nameof(elementOffset));
			IL.Emit.Sizeof(typeof(T));
			IL.Emit.Conv_I();
			IL.Emit.Mul();
			IL.Emit.Add();
			return IL.ReturnPointer();
		}


		[NativeFunction]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref T Add<T>(ref T source, IntPtr elementOffset)
		{
			IL.Emit.Ldarg(nameof(source));
			IL.Emit.Ldarg(nameof(elementOffset));
			IL.Emit.Sizeof(typeof(T));
			IL.Emit.Mul();
			IL.Emit.Add();
			return ref IL.ReturnRef<T>();
		}


		[NativeFunction]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref T AddByteOffset<T>(ref T source, IntPtr byteOffset)
		{
			IL.Emit.Ldarg(nameof(source));
			IL.Emit.Ldarg(nameof(byteOffset));
			IL.Emit.Add();
			return ref IL.ReturnRef<T>();
		}


		[NativeFunction]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref T Subtract<T>(ref T source, int elementOffset)
		{
			IL.Emit.Ldarg(nameof(source));
			IL.Emit.Ldarg(nameof(elementOffset));
			IL.Emit.Sizeof(typeof(T));
			IL.Emit.Conv_I();
			IL.Emit.Mul();
			IL.Emit.Sub();
			return ref IL.ReturnRef<T>();
		}


		[NativeFunction]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void* Subtract<T>(void* source, int elementOffset)
		{
			IL.Emit.Ldarg(nameof(source));
			IL.Emit.Ldarg(nameof(elementOffset));
			IL.Emit.Sizeof(typeof(T));
			IL.Emit.Conv_I();
			IL.Emit.Mul();
			IL.Emit.Sub();
			return IL.ReturnPointer();
		}


		[NativeFunction]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref T Subtract<T>(ref T source, IntPtr elementOffset)
		{
			IL.Emit.Ldarg(nameof(source));
			IL.Emit.Ldarg(nameof(elementOffset));
			IL.Emit.Sizeof(typeof(T));
			IL.Emit.Mul();
			IL.Emit.Sub();
			return ref IL.ReturnRef<T>();
		}


		[NativeFunction]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref T SubtractByteOffset<T>(ref T source, IntPtr byteOffset)
		{
			IL.Emit.Ldarg(nameof(source));
			IL.Emit.Ldarg(nameof(byteOffset));
			IL.Emit.Sub();
			return ref IL.ReturnRef<T>();
		}


		[NativeFunction]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IntPtr ByteOffset<T>(ref T origin, ref T target)
		{
			IL.Emit.Ldarg(nameof(target));
			IL.Emit.Ldarg(nameof(origin));
			IL.Emit.Sub();
			return IL.Return<IntPtr>();
		}


		[NativeFunction]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool AreSame<T>(ref T left, ref T right)
		{
			IL.Emit.Ldarg(nameof(left));
			IL.Emit.Ldarg(nameof(right));
			IL.Emit.Ceq();
			return IL.Return<bool>();
		}


		[NativeFunction]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsAddressGreaterThan<T>(ref T left, ref T right)
		{
			IL.Emit.Ldarg(nameof(left));
			IL.Emit.Ldarg(nameof(right));
			IL.Emit.Cgt_Un();
			return IL.Return<bool>();
		}


		[NativeFunction]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsAddressLessThan<T>(ref T left, ref T right)
		{
			IL.Emit.Ldarg(nameof(left));
			IL.Emit.Ldarg(nameof(right));
			IL.Emit.Clt_Un();
			return IL.Return<bool>();
		}

		#endregion
	}
}
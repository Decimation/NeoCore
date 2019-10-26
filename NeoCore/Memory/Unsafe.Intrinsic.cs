using System;
using System.Drawing;
using System.Runtime.CompilerServices;
using InlineIL;
using NeoCore.Interop.Attributes;

namespace NeoCore.Memory
{
	public static unsafe partial class Unsafe
	{
		#region Unsafe

		// https://github.com/ltrzesniewski/InlineIL.Fody/blob/master/src/InlineIL.Examples/Unsafe.cs
		// https://github.com/dotnet/corefx/blob/master/src/System.Runtime.CompilerServices.Unsafe/src/System.Runtime.CompilerServices.Unsafe.il
		// https://github.com/dotnet/corefx/blob/master/src/Common/src/CoreLib/Internal/Runtime/CompilerServices/Unsafe.cs


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
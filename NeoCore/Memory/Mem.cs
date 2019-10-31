using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using NeoCore.Assets;
using NeoCore.CoreClr;
using NeoCore.CoreClr.Components.Support;
using NeoCore.Interop;
using NeoCore.Memory.Pointers;
using NeoCore.Utilities;
using NeoCore.Utilities.Extensions;

namespace NeoCore.Memory
{
	public static unsafe partial class Mem
	{
		#region Fields

		/// <summary>
		/// Size of a pointer. Equals <see cref="IntPtr.Size"/>.
		/// </summary>
		public static readonly int Size = IntPtr.Size;

		/// <summary>
		/// Determines whether this process is 64-bit.
		/// </summary>
		public static readonly bool Is64Bit = Size == sizeof(long) && Environment.Is64BitProcess;

		/// <summary>
		/// Represents a <c>null</c> <see cref="Pointer{T}"/>. Equivalent to <see cref="IntPtr.Zero"/>.
		/// </summary>
		public static readonly Pointer<byte> Nullptr = null;

		#endregion

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

		public static T ReadStructure<T>(byte[] bytes) where T : struct
		{
			// Pin the managed memory while, copy it out the data, then unpin it

			//	|            Method |     Mean |   Error |  StdDev |
			//	|------------------ |---------:|--------:|--------:|
			//	|     ReadStructure | 166.8 ns | 2.02 ns | 1.89 ns |
			//	| ReadStructureFast | 113.3 ns | 1.03 ns | 0.91 ns |

			byte* handle = stackalloc byte[bytes.Length];
			Marshal.Copy(bytes, 0, (IntPtr) handle, bytes.Length);

			var value = (T) Marshal.PtrToStructure((IntPtr) handle, typeof(T));

			return value;
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
			if (!Runtime.Info.IsStruct(value)) {
				int           size = Unsafe.SizeOf(value, SizeOfOptions.Data);
				Pointer<byte> ptr  = Unsafe.AddressOfFields(ref value);
				ptr.Cast().Clear(size);
			}

			value = default;
		}
	}
}
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using NeoCore.Assets;
using NeoCore.CoreClr;
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
		
		/// <summary>
		/// Reads in a block from a file and converts it to the struct
		/// type specified by the template parameter
		/// </summary>
		public static T ReadStructure<T>(byte[] bytes) where T : struct
		{
			// Pin the managed memory while, copy it out the data, then unpin it
			// todo: this can be done without a GCHandle
			var handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
			var value  = (T) Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
			handle.Free();

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

		public static int OffsetOf<T>(string name, OffsetOfType type, bool isProperty = false) =>
			OffsetOf(typeof(T), name, type, isProperty);

		public static int OffsetOf(Type t, string name, OffsetOfType type, bool isProperty = false)
		{
			if (isProperty) {
				name = Format.GetBackingFieldName(name);
			}

			switch (type) {
				case OffsetOfType.Marshal:
					return (int) Marshal.OffsetOf(t, name);
				case OffsetOfType.Managed:
					var mt = t.AsMetaType();
					return mt[name].Offset;
				default:
					throw new ArgumentOutOfRangeException(nameof(type), type, null);
			}
		}
	}
}
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

		public static readonly bool IsBigEndian = !BitConverter.IsLittleEndian;

		/// <summary>
		/// Represents a <c>null</c> <see cref="Pointer{T}"/>. Equivalent to <see cref="IntPtr.Zero"/>.
		/// </summary>
		public static readonly Pointer<byte> Nullptr = null;

		#endregion


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
			if (r.HasAddresses) {
				return IsAddressInRange(p, r.Low, r.High);
			}
			else if (r.HasSize) {
				return IsAddressInRange(p, r.Low, r.Size);
			}
			throw new InvalidOperationException();
		}
		
		/// <param name="p">Operand</param>
		/// <param name="lo">Start address</param>
		/// <param name="hi">End address</param>
		public static bool IsAddressInRange(Pointer<byte> p, Pointer<byte> lo, Pointer<byte> hi)
		{
			// if ((ptrStack < stackBase) && (ptrStack > (stackBase - stackSize)))
			// (p >= regionStart && p < regionStart + regionSize) ;
			// return target >= start && target < end;
			// return m_CacheStackLimit < addr && addr <= m_CacheStackBase;
			// if (!((object < g_gc_highest_address) && (object >= g_gc_lowest_address)))
			// return max.ToInt64() < p.ToInt64() && p.ToInt64() <= min.ToInt64();

			return p < hi && p >= lo;
		}

		public static bool IsAddressInRange(Pointer<byte> p, Pointer<byte> lo, long size)
		{
			// if ((ptrStack < stackBase) && (ptrStack > (stackBase - stackSize)))
			// (p >= regionStart && p < regionStart + regionSize) ;
			// return target >= start && target < end;
			// return m_CacheStackLimit < addr && addr <= m_CacheStackBase;
			// if (!((object < g_gc_highest_address) && (object >= g_gc_lowest_address)))
			// return max.ToInt64() < p.ToInt64() && p.ToInt64() <= min.ToInt64();

			
			return (p >= lo && p < lo + size) ;
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
				int           size = Unsafe.SizeOf(value, SizeOfOptions.Data);
				Pointer<byte> ptr  = Unsafe.AddressOfFields(ref value);
				ptr.Cast().Clear(size);
			}

			value = default;
		}
	}
	

	
}
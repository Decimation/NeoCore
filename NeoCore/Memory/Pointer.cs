#region

using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using NeoCore.CoreClr;
using NeoCore.Interop.Attributes;
using NeoCore.Utilities;
using static NeoCore.Assets.Constants.Defaults;

#endregion

namespace NeoCore.Memory
{
	/// <summary>
	///     <para>Represents a native pointer. Equals the size of <see cref="P:System.IntPtr.Size" />.</para>
	///     <para>Can be represented as a native pointer in memory. </para>
	///     <para>
	///         Supports pointer arithmetic, reading/writing different any type, and more.
	///     </para>
	///     <list type="bullet">
	///         <item>
	///             <description>No bounds checking</description>
	///         </item>
	///         <item>
	///             <description>Minimum type safety</description>
	///         </item>
	///     </list>
	/// </summary>
	/// <typeparam name="T">Pointer element type</typeparam>
	public unsafe struct Pointer<T>
	{
		/// <summary>
		/// Internal pointer value.
		/// </summary>
		private void* m_value;

		#region Properties

		/// <summary>
		///     Size of element type <typeparamref name="T" />.
		/// </summary>
		public int ElementSize => Unsafe.SizeOf<T>();

		/// <summary>
		///     Indexes <see cref="Address" /> as a reference.
		/// </summary>
		public ref T this[int index] => ref AsRef(index);

		/// <summary>
		///     Returns the current value as a reference.
		/// </summary>
		public ref T Reference => ref AsRef();

		/// <summary>
		///     Dereferences the pointer as the specified type.
		/// </summary>
		public T Value {
			get => Read();
			set => Write(value);
		}

		/// <summary>
		///     Address being pointed to.
		/// </summary>
		public IntPtr Address {
			get => (IntPtr) m_value;
			set => m_value = (void*) value;
		}

		/// <summary>
		///     Whether <see cref="Address" /> is <c>null</c> (<see cref="IntPtr.Zero" />).
		/// </summary>
		public bool IsNull => this == Mem.Nullptr;

		#endregion

		#region Constructors

		public Pointer(void* value)
		{
			m_value = value;
		}

		public Pointer(IntPtr value) : this(value.ToPointer()) { }

		#endregion

		#region Implicit / explicit conversions

		public static explicit operator Pointer<T>(ulong ul) => new Pointer<T>((void*) ul);

		public static explicit operator void*(Pointer<T> ptr) => ptr.ToPointer();

		public static explicit operator long(Pointer<T> ptr) => ptr.ToInt64();

		public static explicit operator ulong(Pointer<T> ptr) => (ulong) ptr.ToInt64();

		public static explicit operator Pointer<byte>(Pointer<T> ptr) => ptr.ToPointer();

		public static implicit operator Pointer<T>(void* value) => new Pointer<T>(value);

		public static implicit operator Pointer<T>(IntPtr value) => new Pointer<T>(value);

		public static implicit operator Pointer<T>(Pointer<byte> ptr) => ptr.Address;

		public static explicit operator Pointer<T>(long value) => new Pointer<T>((void*) value);

		#endregion

		#region Equality operators

		/// <summary>
		///     Checks to see if <see cref="other" /> is equal to the current instance.
		/// </summary>
		/// <param name="other">Other <see cref="Pointer{T}" />.</param>
		/// <returns></returns>
		public bool Equals(Pointer<T> other) => Address == other.Address;

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
				return false;

			return obj is Pointer<T> pointer && Equals(pointer);
		}

		public override int GetHashCode()
		{
			// ReSharper disable once NonReadonlyMemberInGetHashCode
			return unchecked((int) (long) m_value);
		}

		public static bool operator ==(Pointer<T> left, Pointer<T> right) => left.Equals(right);

		public static bool operator !=(Pointer<T> left, Pointer<T> right) => !left.Equals(right);

		#endregion

		#region Arithmetic

		/// <summary>
		///     Increment <see cref="Address" /> by the specified number of bytes
		/// </summary>
		/// <param name="byteCnt">Number of bytes to add</param>
		/// <returns>
		///     A new <see cref="Pointer{T}"/> with <paramref name="byteCnt"/> bytes added
		/// </returns>
		[Pure]
		public Pointer<T> Add(long byteCnt = 1)
		{
			long val = ToInt64() + byteCnt;
			return (void*) val;
		}


		/// <summary>
		///     Decrement <see cref="Address" /> by the specified number of bytes
		/// </summary>
		/// <param name="byteCnt">Number of bytes to subtract</param>
		/// <returns>
		///     A new <see cref="Pointer{T}"/> with <paramref name="byteCnt"/> bytes subtracted
		/// </returns>
		[Pure]
		public Pointer<T> Subtract(long byteCnt = 1) => Add(-byteCnt);

		public static Pointer<T> operator +(Pointer<T> left, long right)
		{
			return (void*) (left.ToInt64() + right);
		}

		public static Pointer<T> operator -(Pointer<T> left, long right)
		{
			return (void*) (left.ToInt64() - right);
		}

		[Pure]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void* Offset(int elemCnt) => (void*) ((long) m_value + (Mem.FlatSize(ElementSize, elemCnt)));

		[Pure]
		public Pointer<T> AddressOfIndex(int index) => Offset(index);

		#region Operators

		/// <summary>
		///     Increment <see cref="Address" /> by the specified number of elements
		/// </summary>
		/// <param name="elemCnt">Number of elements</param>
		/// <returns>
		///     A new <see cref="Pointer{T}"/> with <paramref name="elemCnt"/> elements incremented
		/// </returns>
		[Pure]
		public Pointer<T> Increment(int elemCnt = DEF_ELEM_CNT) => Offset(elemCnt);


		/// <summary>
		///     Decrement <see cref="Address" /> by the specified number of elements
		/// </summary>
		/// <param name="elemCnt">Number of elements</param>
		/// <returns>
		///     A new <see cref="Pointer{T}"/> with <paramref name="elemCnt"/> elements decremented
		/// </returns>
		[Pure]
		public Pointer<T> Decrement(int elemCnt = DEF_ELEM_CNT) => Increment(-elemCnt);

		/// <summary>
		///     Increments the <see cref="Address" /> by the specified number of elements.
		///     <remarks>
		///         Equal to <see cref="Pointer{T}.Increment" />
		///     </remarks>
		/// </summary>
		/// <param name="ptr">
		///     <see cref="Pointer{T}" />
		/// </param>
		/// <param name="i">Number of elements (<see cref="ElementSize" />)</param>
		public static Pointer<T> operator +(Pointer<T> ptr, int i) => ptr.Increment(i);

		/// <summary>
		///     Decrements the <see cref="Address" /> by the specified number of elements.
		///     <remarks>
		///         Equal to <see cref="Pointer{T}.Decrement" />
		///     </remarks>
		/// </summary>
		/// <param name="ptr">
		///     <see cref="Pointer{T}" />
		/// </param>
		/// <param name="i">Number of elements (<see cref="ElementSize" />)</param>
		public static Pointer<T> operator -(Pointer<T> ptr, int i) => ptr.Decrement(i);

		/// <summary>
		///     Increments the <see cref="Pointer{T}" /> by one element.
		/// </summary>
		/// <param name="ptr">
		///     <see cref="Pointer{T}" />
		/// </param>
		/// <returns>The offset <see cref="Address" /></returns>
		public static Pointer<T> operator ++(Pointer<T> ptr) => ptr.Increment();

		/// <summary>
		///     Decrements the <see cref="Pointer{T}" /> by one element.
		/// </summary>
		/// <param name="ptr">
		///     <see cref="Pointer{T}" />
		/// </param>
		/// <returns>The offset <see cref="Address" /></returns>
		public static Pointer<T> operator --(Pointer<T> ptr) => ptr.Decrement();

		#endregion

		#endregion

		#region Read / write

		/// <summary>
		///     Writes a value of type <typeparamref name="T" /> to <see cref="Address" />.
		/// </summary>
		/// <param name="value">Value to write.</param>
		/// <param name="elemOffset">Element offset (in terms of type <typeparamref name="T" />).</param>
		public void Write(T value, int elemOffset = DEF_OFFSET) => Unsafe.Write(Offset(elemOffset), value);


		/// <summary>
		///     Reads a value of type <typeparamref name="T" /> from <see cref="Address" />.
		/// </summary>
		/// <param name="elemOffset">Element offset (in terms of type <typeparamref name="T" />).</param>
		/// <returns>The value read from the offset <see cref="Address" />.</returns>
		[Pure]
		public T Read(int elemOffset = DEF_OFFSET) => Unsafe.Read<T>(Offset(elemOffset));

		/// <summary>
		///     Reinterprets <see cref="Address" /> as a reference to a value of type <typeparamref name="T" />.
		/// </summary>
		/// <param name="elemOffset">Element offset (in terms of type <typeparamref name="T" />).</param>
		/// <returns>A reference to a value of type <typeparamref name="T" />.</returns>
		[Pure]
		public ref T AsRef(int elemOffset = DEF_OFFSET) => ref Unsafe.AsRef<T>(Offset(elemOffset));


		#region Pointer

		[Pure]
		public Pointer<byte> ReadPointer(int elemOffset = DEF_OFFSET) => ReadPointer<byte>(elemOffset);

		[Pure]
		public Pointer<TType> ReadPointer<TType>(int elemOffset = DEF_OFFSET)
		{
			return Cast<Pointer<TType>>().Read(elemOffset);
		}

		public void WritePointer<TType>(Pointer<TType> ptr, int elemOffset = DEF_OFFSET)
		{
			Cast<Pointer<TType>>().Write(ptr, elemOffset);
		}

		#endregion

		#endregion

		#region Copy

		/// <summary>
		///     Copies <paramref name="elemCnt" /> elements into an array of type <typeparamref name="T" />,
		///     starting from index <paramref name="startIndex" />
		/// </summary>
		/// <param name="startIndex">Index to begin copying from</param>
		/// <param name="elemCnt">Number of elements to copy</param>
		/// <returns>
		///     An array of length <paramref name="elemCnt" /> of type <typeparamref name="T" /> copied from
		///     the current pointer
		/// </returns>
		[Pure]
		public T[] Copy(int startIndex, int elemCnt)
		{
			var rg = new T[elemCnt];
			for (int i = startIndex; i < elemCnt + startIndex; i++)
				rg[i - startIndex] = this[i];

			return rg;
		}

		/// <summary>
		///     Copies <paramref name="elemCnt" /> elements into an array of type <typeparamref name="T" />,
		///     starting from index 0.
		/// </summary>
		/// <param name="elemCnt">Number of elements to copy</param>
		/// <returns>
		///     An array of length <paramref name="elemCnt" /> of type <typeparamref name="T" /> copied from
		///     the current pointer
		/// </returns>
		[Pure]
		public T[] Copy(int elemCnt) => Copy(0, elemCnt);

		#endregion

		#region Cast

		/// <summary>
		///     Creates a new <see cref="Pointer{T}" /> of type <typeparamref name="TNew" />, pointing to
		///     <see cref="Address" />
		/// </summary>
		/// <typeparam name="TNew">Type to point to</typeparam>
		/// <returns>A new <see cref="Pointer{T}" /> of type <typeparamref name="TNew" /></returns>
		public Pointer<TNew> Cast<TNew>() => m_value;

		/// <summary>
		///     Creates a new <see cref="Pointer{T}" /> of type <see cref="Byte"/>, pointing to
		///     <see cref="Address" />
		/// </summary>
		/// <returns>A new <see cref="Pointer{T}" /> of type <see cref="Byte"/></returns>
		public Pointer<byte> Cast() => Cast<byte>();

		/// <summary>
		///     Creates a native pointer of type <typeparamref name="TUnmanaged"/>, pointing to
		///     <see cref="Address" />
		/// </summary>
		/// <returns>A native pointer of type <typeparamref name="TUnmanaged"/></returns>
		[Pure]
		public TUnmanaged* ToPointer<TUnmanaged>() where TUnmanaged : unmanaged => (TUnmanaged*) m_value;

		/// <summary>
		///     Creates a native <c>void*</c> pointer, pointing to <see cref="Address" />
		/// </summary>
		/// <returns>A native <c>void*</c> pointer</returns>
		[Pure]
		public void* ToPointer() => m_value;


		[Pure]
		public long ToInt64() => (long) m_value;

		[Pure]
		public int ToInt32() => (int) m_value;

		#endregion

		public override string ToString()
		{
			return Format.AsHex(m_value);
		}
	}
}
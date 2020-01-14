using System;
using Memkit;
using Memkit.Pointers;
using NeoCore.Win32.Attributes;

namespace NeoCore.Memory.ExtraPointers
{
	// todo: WIP

	[NativeStructure]
	public unsafe struct RelativePointer<T> : IRelativePointer<T> where T : unmanaged
	{
		public ulong Value { get; }

		public T* NativeValue => (T*) Value;

		public RelativePointer(ulong delta)
		{
			Value = delta;
		}

		/// <summary>
		/// Returns value of the encoded pointer. Assumes that the pointer is not NULL.
		/// </summary>
		public Pointer<T> GetValue(ulong value)
		{
			// return dac_cast<PTR_TYPE>(base + m_delta);

			return (Pointer<T>) (Value + value);
		}

		public override string ToString()
		{
			return String.Format("{0:X} d", Value);
		}
	}
}
using System;
using NeoCore.Interop.Attributes;

namespace NeoCore.Memory.Pointers
{
	// todo: WIP

	[NativeStructure]
	public unsafe struct RelativePointer<T> : IRelativePointer<T>
	{
		public ulong Value { get; }

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
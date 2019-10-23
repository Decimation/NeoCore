using System;

namespace NeoCore.CoreClr.Support
{
	// todo: WIP
	
	/// <summary>
	/// Alias: TADDR
	/// </summary>
	public struct QInt
	{
		// Define TADDR as a non-pointer value so use of it as a pointer
		// will not work properly.  Define it as unsigned so
		// pointer comparisons aren't affected by sign.
		// This requires special casting to ULONG64 to sign-extend if necessary.
		//
		// typedef ULONG_PTR TADDR;

		// typedef unsigned __int3264 ULONG_PTR;
		// 
		// __int3264 can be represented with an IntPtr

		// https://docs.microsoft.com/en-us/openspecs/windows_protocols/ms-dtyp/21eec394-630d-49ed-8b4a-ab74a1614611

		private UIntPtr m_value;

		public ulong Value => m_value.ToUInt64();

		public QInt(ulong value) => m_value = (UIntPtr) value;

		public QInt(UIntPtr value) : this((ulong) value) { }

		public static implicit operator QInt(ulong value) => new QInt(value);

		public static implicit operator QInt(UIntPtr value) => new QInt(value);

		public static implicit operator QInt(int value) => new QInt((ulong) value);

		public static QInt operator +(QInt value, QInt value2) => value.Value + value2.Value;

		public static QInt operator -(QInt value, QInt value2) => value.Value - value2.Value;
		
		public static QInt operator &(QInt value, QInt value2) => value.Value & value2.Value;
		
		public static unsafe explicit operator QInt(void* ptr) => new QInt((UIntPtr) ptr);
		
		public static unsafe explicit operator ulong(QInt q) => q.Value;

		public bool Equals(QInt other)
		{
			return m_value.Equals(other.m_value);
		}

		public override bool Equals(object obj)
		{
			return obj is QInt other && Equals(other);
		}

		public override int GetHashCode()
		{
			return m_value.GetHashCode();
		}

		public static bool operator ==(QInt left, QInt right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(QInt left, QInt right)
		{
			return !left.Equals(right);
		}

		public override string ToString() => Value.ToString();
	}
}
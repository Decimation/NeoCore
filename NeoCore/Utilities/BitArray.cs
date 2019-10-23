namespace NeoCore.Utilities
{
	public struct BitArray
	{
		public uint Value { get; }

		public BitArray(uint value) => Value = value;
		
		public uint this[int index, int bits = 1] {
			get { return (uint) Bits.ReadBits(Value, index, bits); }
		}
		
		public static implicit operator BitArray(uint value) => new BitArray(value);

		public static explicit operator uint(BitArray value) => value.Value;

		public override string ToString()
		{
			return Value.ToString();
		}
	}
}
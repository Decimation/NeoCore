namespace NeoCore.Utilities
{
	public readonly struct BitArray
	{
		public int Value { get; }

		public BitArray(int value) => Value = value;

		public uint this[int index, int bits = 1] => (uint) Bits.ReadBits(Value, index, bits);

		public static implicit operator BitArray(int value) => new BitArray(value);

		public static explicit operator int(BitArray value) => value.Value;

		public override string ToString() => Value.ToString();
	}
}
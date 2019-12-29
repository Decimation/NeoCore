using System;
using NeoCore.Utilities;

namespace NeoCore.Import.Providers
{
	public readonly struct ImageRecordEntry
	{
		public string Name { get; }

		public EntryType Type { get; }

		/// <summary>
		/// <see cref="byte"/> array if <see cref="Type"/> is <see cref="EntryType.Signature"/>,
		/// <see cref="long"/> if <see cref="Type"/> is <see cref="EntryType.Offset"/>
		/// </summary>
		public object Value { get; }


		internal ImageRecordEntry(string name, EntryType t, object v)
		{
			Name  = name;
			Type  = t;
			Value = v;
		}

		public override string ToString()
		{
			string       valStr  = null;
			const string FMT_HEX = "X";

			switch (Type) {
				case EntryType.Signature:
					var rg = Value as byte[];
					valStr = rg.FormatJoin(FMT_HEX);
					break;
				case EntryType.Offset:
					long l = (long) Value;
					valStr = l.ToString(FMT_HEX);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			return String.Format("{0}: {1} ({2})", Name, valStr, Type);
		}
	}

	public enum EntryType
	{
		Signature,
		Offset
	}
}
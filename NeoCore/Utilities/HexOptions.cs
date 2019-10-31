using System;

namespace NeoCore.Utilities
{
	[Flags]
	public enum HexOptions
	{
		None = 0,

		Prefix = 1,

		Lowercase = 1 << 1,


		Default = Prefix
	}
}
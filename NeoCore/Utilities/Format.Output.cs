using System;

namespace NeoCore.Utilities
{
	public static partial class Format
	{
		public static class Output
		{
			public static void WriteLineQuick(params object[] args)
			{
				Console.WriteLine(String.Join(Format.Constants.JOIN_SPACE, args));
			}
		}
	}
}
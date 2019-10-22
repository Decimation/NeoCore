using System;
using NeoCore;

namespace Test
{
	internal static unsafe class Program
	{
		private static void Main(string[] args)
		{
			Console.WriteLine("Hello World!");
			Console.WriteLine(Runtime.Info.CurrentFramework);

			int i = 256;
			Pointer<int> p = &i;
			Console.WriteLine(p);
			Console.WriteLine("{0:X}", new IntPtr(&i).ToInt64());
		}
	}
}
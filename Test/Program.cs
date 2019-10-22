#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using NeoCore;
using NeoCore.Memory;
using NeoCore.Utilities;

#endregion

namespace Test
{
	internal static unsafe class Program
	{
		private static void Main(string[] args)
		{
			int          i = Int32.MaxValue;
			Pointer<int> p = &i;
			Console.WriteLine(p.Value);

			var p2 = p.Cast<ushort>();
			Console.WriteLine(p2.Value);
			p2++;
			Console.WriteLine(p2.Value);
			Console.WriteLine(++p2.Value);

			var rg = BitConverter.GetBytes(i);
			Console.WriteLine(Format.Collections.FormatJoin(rg, "X"));

			Console.WriteLine(Format.Collections.FuncJoin(rg, b => b + "b"));
		}
	}
}
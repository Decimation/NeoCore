#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using NeoCore;
using NeoCore.CoreClr;
using NeoCore.Memory;
using NeoCore.Utilities;
using NeoCore.Utilities.Diagnostics;

#endregion

namespace Test
{
	internal static unsafe class Program
	{
		private static void Main(string[] args)
		{
			Console.WriteLine(RuntimeEnvironment.GetRuntimeDirectory());

			foreach (ProcessModule module in Process.GetCurrentProcess().Modules) {
				Console.WriteLine("{0}: {1}", module.FileName, module.ModuleName);
			}
		}
	}
}
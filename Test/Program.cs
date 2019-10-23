#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using NeoCore;
using NeoCore.Assets;
using NeoCore.CoreClr;
using NeoCore.CoreClr.Meta;
using NeoCore.Memory;
using NeoCore.Utilities;
using NeoCore.Utilities.Diagnostics;

#endregion

namespace Test
{
	internal static unsafe class Program
	{
		public static void Hi()
		{
			Console.WriteLine("g");
		}
		
		private static void Main(string[] args)
		{
			
			var t = (MetaType) typeof(string);
			Console.WriteLine(t);
			Console.WriteLine(t);
		}
	}
}
#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using NeoCore;
using NeoCore.Assets;
using NeoCore.CoreClr;
using NeoCore.CoreClr.Meta;
using NeoCore.Import;
using NeoCore.Import.Attributes;
using NeoCore.Memory;
using NeoCore.Memory.Pointers;
using NeoCore.Model;
using NeoCore.Utilities;
using NeoCore.Utilities.Diagnostics;
using NeoCore.Utilities.Extensions;

#endregion

namespace Test
{
	internal static unsafe class Program
	{
		
		private static void Main(string[] args)
		{
			Console.WriteLine(Runtime.Info.IsPinnable("foo"));
			Console.WriteLine(Runtime.Info.IsPinnable<object>(null));

			var rg = new[] {"foo"};
			Console.WriteLine(Runtime.Info.IsPinnable(rg));

			GCHandle.Alloc(rg, GCHandleType.Pinned);
		}
	}
}
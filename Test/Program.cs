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
		public struct BlittableStruct
		{
			public int IntField;

			public void Hello() { }
		}
		
		private static void Main(string[] args)
		{
			
			var b = new BlittableStruct();
			var rg = new[] {new BlittableStruct()};
			
			Console.WriteLine(Runtime.Info.IsPinnableFast(b));
			Console.WriteLine(Runtime.Info.IsPinnableFast(new[]{1}));
			Console.WriteLine(Runtime.Info.IsPinnableFast(rg));
			
			var h=GCHandle.Alloc(rg, GCHandleType.Pinned);
			Console.WriteLine(h.IsAllocated);
		}
	}
}
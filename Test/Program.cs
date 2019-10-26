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
			
			var rg = new[] {1};
			GCHandle.Alloc(rg, GCHandleType.Pinned);

			var mt = rg.GetType().AsMetaType();
			var ee = mt.Value.Reference.EEClass;
			Console.WriteLine(mt.ElementTypeHandle);
			Console.WriteLine(ee.Reference.AsArrayClass.Reference.Rank);
			
			Console.WriteLine(Runtime.Info.IsPinnableAlt(rg));

		}
	}
}
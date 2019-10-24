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
using NeoCore.CoreClr.Support;
using NeoCore.FastReflection;
using NeoCore.Import;
using NeoCore.Import.Attributes;
using NeoCore.Memory;
using NeoCore.Memory.Pointers;
using NeoCore.Model;
using NeoCore.Utilities;
using NeoCore.Utilities.Diagnostics;

#endregion

namespace Test
{
	internal static unsafe class Program
	{
		public class TestObject
		{
			private protected string StringField;

			public void Hello() { }
		}


		private static void Main(string[] args)
		{
			var t = (MetaType) typeof(TestObject);
			var ee = t.Value.Reference.EEClass;
			Console.WriteLine(ee.Reference.NumInstanceFields);
			Console.WriteLine(ee.Reference.NumStaticFields);
			Console.WriteLine(ee.Reference.NumMethods);
			Console.WriteLine(ee.Reference.NumNonVirtualSlots);
		}
	}
}
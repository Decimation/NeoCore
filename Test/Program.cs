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
using NeoCore.CoreClr.VM;
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
			var mt = (MetaType) typeof(TestObject);
			Console.WriteLine(mt);

			Console.WriteLine(Globals.GCHeap);
		}
	}
}
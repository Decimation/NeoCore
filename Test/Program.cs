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
		interface IInterface
		{
			string Name { get; }
		}

		public struct TestObject : IInterface
		{
			private string Field { get; }

			public void Hello() { }

			public string Name => "Foo";
		}


		private static void Main(string[] args)
		{
			var t = new MetaType(typeof(TestObject));
			Console.WriteLine(t);
		}
	}
}
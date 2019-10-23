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
using NeoCore.CoreClr.Metadata;
using NeoCore.CoreClr.Support;
using NeoCore.Import;
using NeoCore.Memory;
using NeoCore.Utilities;
using NeoCore.Utilities.Diagnostics;

#endregion

namespace Test
{
	internal static unsafe class Program
	{
		class MyClass
		{
			public int Field;

			public void Hi() { }

			public int Value {
				get { return 1; }
			}
		}


		[StructLayout(LayoutKind.Sequential)]
		struct MyStruct
		{
			internal MethodTable* Field { get; }
		}

		private static void Main(string[] args)
		{
			Resources.SetupAll();

			// 7FFBD9A975C8
			var mt = (MetaType) typeof(MyClass);
			var fn = mt.GetMethod("Hi");
			Console.WriteLine(fn.EnclosingType);

			var fld = mt.GetField("Field");
			Console.WriteLine(fld);

			Console.WriteLine(fn.Value.Reference.MethodDescChunk.Reference.FlagsAndTokenRange);
		}
	}
}
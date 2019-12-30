#region

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading;
using JetBrains.Annotations;
using NeoCore;
using NeoCore.CoreClr;
using NeoCore.CoreClr.Meta;
using NeoCore.CoreClr.Meta.Base;
using NeoCore.Import;
using NeoCore.Import.Attributes;
using NeoCore.Import.Providers;
using NeoCore.Interop;
using NeoCore.Interop.Attributes;
using NeoCore.Memory;
using NeoCore.Memory.Pointers;
using NeoCore.Model;
using NeoCore.Support;
using NeoCore.Utilities;
using NeoCore.Utilities.Diagnostics;
using NeoCore.Utilities.Extensions;
using Serilog.Core;
using Unsafe = NeoCore.Memory.Unsafe;
using static NeoCore.Utilities.EasyReflection;

#endregion

namespace Test
{
	// nuget pack -Prop Configuration=Release

	public static unsafe class Program
	{
		private static void Main(string[] args)
		{
			var t = typeof(MyStruct[]).AsMetaType();
			Console.WriteLine(t.DebugTable);

			var f = t["a"];
			Console.WriteLine(f);


			var m = t.GetMethod("func");
			Console.WriteLine(m);
			m.Prepare();
			Console.WriteLine(m.NativeCode);
		}

		class MyClass
		{
			private int  b;
			public  void f() { }
		}

		class MyStruct : MyClass
		{
			public int a;

			public void func() { }
		}
	}
}
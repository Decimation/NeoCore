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

			
			
		}

		static void fn()
		{
			int           i  = 256;
			Pointer<byte> p  = &i;
			Pointer<byte> lo = &i;
			Pointer<byte> hi = ((byte*) &i) + 3;

			Console.WriteLine("lo: {0}", lo);
			Console.WriteLine("hi: {0}", hi);
			Console.WriteLine("p: {0}", p);
			Console.WriteLine("size: {0}", sizeof(int));
			
			Console.WriteLine(Mem.IsAddressInRange(p, lo,hi));
			Console.WriteLine(Mem.IsAddressInRange(p+3, lo,hi));
			Console.WriteLine(Mem.IsAddressInRange(p+3,lo,4));
			
			var r1 = new Region(lo, hi);
			Console.WriteLine(r1);
			
			var r2 = new Region(lo, 4);
			Console.WriteLine(r2);
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
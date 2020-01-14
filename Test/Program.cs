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
using Memkit;
using Memkit.Model;
using Memkit.Pointers;
using Memkit.Utilities;
using NeoCore;
using NeoCore.CoreClr;
using NeoCore.CoreClr.Meta;
using NeoCore.CoreClr.Meta.Base;
using NeoCore.Import;
using NeoCore.Import.Attributes;
using NeoCore.Import.Providers;
using NeoCore.Model;
using NeoCore.Support;
using NeoCore.Utilities;
using NeoCore.Utilities.Diagnostics;
using NeoCore.Win32;
using Serilog.Core;
using static NeoCore.Utilities.EasyReflection;

#endregion

namespace Test
{
	// nuget pack -Prop Configuration=Release
	
	// todo: rewrite entire library :/

	public static unsafe class Program
	{
		
		private static void Main(string[] args)
		{
			Console.WriteLine(UniqueMember.Get);
			Console.WriteLine(typeof(MyStructx).GetAllFields()[0].FieldType.Name);
			Console.WriteLine(UniqueMember.FixedBufferType.GetName("buffer"));
			
		}

		struct MyStructx
		{
			private fixed int buffer[5];
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
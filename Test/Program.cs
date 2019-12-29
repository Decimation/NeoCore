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
using NeoCore.Import.Providers.ImageIndex;
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


	struct MyStruct
	{
		public int a;
	}

	public static unsafe class Program
	{
		private static void Main(string[] args)
		{
			
			var clr = new Region(Resources.Clr.Module.BaseAddress, Resources.Clr.Module.ModuleMemorySize);
			var ss  = new SigScanner(clr);
			var p   = ss.FindPattern("48 83 EC 28 F6 C1 02 75 28");
			Console.WriteLine(p);
			Console.WriteLine(p.Copy(5).FormatJoin("X"));

			var actual = Resources.Clr.Imports.GetAddress("FieldDesc::LoadSize");
			Console.WriteLine(actual);
			Console.WriteLine(actual.Copy(5).FormatJoin("X"));
			Console.WriteLine("{0:X}",actual.ToInt64()-Resources.Clr.Module.BaseAddress.ToInt64());

			var idx = @"C:\Users\Deci\RiderProjects\NeoCore\NeoCore\clr_image.txt";
			var v = new ImageIndex(idx, Resources.Clr.Module.BaseAddress);
			foreach (var j in v.Index) {
				Console.WriteLine(j);
			}
		}
	}
}
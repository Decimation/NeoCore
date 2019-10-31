#region

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using NeoCore;
using NeoCore.Assets;
using NeoCore.CoreClr;
using NeoCore.CoreClr.Components;
using NeoCore.CoreClr.Components.Support;
using NeoCore.CoreClr.Components.VM.EE;
using NeoCore.CoreClr.Meta;
using NeoCore.Import;
using NeoCore.Import.Attributes;
using NeoCore.Interop;
using NeoCore.Interop.Structures.Raw.Enums;
using NeoCore.Memory;
using NeoCore.Memory.Pointers;
using NeoCore.Model;
using NeoCore.Utilities;
using NeoCore.Utilities.Diagnostics;
using NeoCore.Utilities.Extensions;
using Serilog.Core;

#endregion

namespace Test
{
	internal static unsafe class Program
	{
		public struct BlittableStruct
		{
			public        int  IntField;
			public static int  StaticIntField = 1;
			public        void Hello() { }
		}


		private static void Main(string[] args)
		{
			string s = "foo";
			var   t  = Runtime.ReadTypeHandle(s);
			Console.WriteLine(t.NativeSize);

			bool b = true;
			var t2 = Runtime.ReadTypeHandle(b);
			Console.WriteLine(sizeof(bool));
			Console.WriteLine(t2.NativeSize);
			Console.WriteLine(Marshal.SizeOf<bool>());
			
		}
	}
}
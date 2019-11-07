#region

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using NeoCore;
using NeoCore.Assets;
using NeoCore.CoreClr;
using NeoCore.CoreClr.Components;
using NeoCore.CoreClr.Components.Support;
using NeoCore.CoreClr.Components.VM;
using NeoCore.CoreClr.Components.VM.EE;
using NeoCore.CoreClr.Components.VM.Jit;
using NeoCore.CoreClr.Meta;
using NeoCore.Import;
using NeoCore.Import.Attributes;
using NeoCore.Interop;
using NeoCore.Interop.Attributes;
using NeoCore.Interop.Structures.Raw.Enums;
using NeoCore.Memory;
using NeoCore.Memory.Pointers;
using NeoCore.Model;
using NeoCore.Utilities;
using NeoCore.Utilities.Diagnostics;
using NeoCore.Utilities.Extensions;
using Serilog.Core;
using Unsafe = NeoCore.Memory.Unsafe;

#endregion

namespace Test
{
	internal static unsafe class Program
	{
		static int Add(int a, int b)
		{
			return a + b;
		}

		private static void Main(string[] args)
		{
			Console.WriteLine(sizeof(CorMethodTiny));
			var m  = typeof(Program).GetAnyMethod(nameof(Add)).AsMetaMethod();
			var il = m.ILHeader;
			var s = il.Value.Reference.Tiny->CodeSize;
			var p = il.Value.Reference.Tiny->Code;
			Console.WriteLine(Format.Collections.ToString(m.MethodInfo.GetMethodBody().GetILAsByteArray()));
			Console.WriteLine(Format.Collections.ToString(p.Copy(s)));
			
		}
	}
}
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
using System.Threading;
using JetBrains.Annotations;
using NeoCore;
using NeoCore.CoreClr;
using NeoCore.CoreClr.Meta;
using NeoCore.CoreClr.Meta.Base;
using NeoCore.Import;
using NeoCore.Import.Attributes;
using NeoCore.Interop;
using NeoCore.Interop.Attributes;
using NeoCore.Memory;
using NeoCore.Memory.Pointers;
using NeoCore.Model;
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
			int i    = 256;
			var info = MemInfo.Inspect(&i);
			Console.WriteLine(info);
		}
	}
}
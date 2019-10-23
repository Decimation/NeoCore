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
			[ThreadStatic]
			public static int Field;

			public void Hi() { }

			public int Value {
				get { return 1; }
			}
		}

		private static void Main(string[] args)
		{
			Resources.SetupAll();

			// 7FFBD9A975C8
			var mt = (MetaType) typeof(MyClass);
			var fn = mt.GetMethod("Hi");
			Console.WriteLine(fn.EnclosingType);

			var fld = mt.GetField("Field");
			Console.WriteLine(fld.Access);

			var u1 = fld.Value.Reference.UInt1;
			Console.WriteLine(u1);
			Console.WriteLine(Convert.ToString(u1, 2));

			var v = u1 & (uint) FieldProperties.Static;
			Console.WriteLine(v);
			var v2 = u1 & (uint) FieldProperties.ThreadLocal;
			Console.WriteLine(v2);
			
			Console.WriteLine(fld.Value.Reference.IsStatic == fld.Value.Reference.Properties.HasFlag(FieldProperties.Static));
			Console.WriteLine(fld.Value.Reference.IsThreadLocal == fld.Value.Reference.Properties.HasFlag(FieldProperties.ThreadLocal));
			Console.WriteLine(fld.Value.Reference.RequiresFullMBValue == fld.Value.Reference.Properties.HasFlag(FieldProperties.RequiresFullMBValue));
			Console.WriteLine(fld.Value.Reference.IsRVA == fld.Value.Reference.Properties.HasFlag(FieldProperties.RVA));
		}
	}
}
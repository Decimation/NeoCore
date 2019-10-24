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
using NeoCore.Import.Attributes;
using NeoCore.Memory;
using NeoCore.Memory.Pointers;
using NeoCore.Model;
using NeoCore.Utilities;
using NeoCore.Utilities.Diagnostics;

#endregion

namespace Test
{
	internal static unsafe class Program
	{
		class MyClass : Releasable
		{
			public int Field;

			public static readonly string Field2 = "11";

			public void Hi() { }

			public int Value {
				get { return 1; }
			}

			public override void Setup()
			{
				base.Setup();
			}

			protected override string Id { get; }
		}

		struct Ptr1<T> : IPointer<T>
		{
			public T Read()
			{
				throw new NotImplementedException();
			}
		}

		class MyClass2
		{
			public IPointer<int> p;
		}
		
		private static void Main(string[] args)
		{
			Resources.SetupAll();
			
			
		}
	}
}
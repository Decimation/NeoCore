#region

using System;
using System.Collections.Generic;
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
using NeoCore.Memory;
using NeoCore.Memory.Pointers;
using NeoCore.Model;
using NeoCore.Utilities;
using NeoCore.Utilities.Diagnostics;
using NeoCore.Utilities.Extensions;

#endregion

namespace Test
{
	internal static unsafe class Program
	{
		public struct BlittableStruct
		{
			public int IntField;

			public void Hello() { }
		}

		private static void Main(string[] args)
		{
			
			var s = new PEHeaderReader(Resources.Clr.LibraryFile.FullName);

			foreach (var header in s.ImageSectionHeaders) {
				Console.WriteLine(header.SectionName);
			}

			Console.WriteLine(s.TimeStamp);
			
			var bs = new BlittableStruct();
			bs.Hello();
		}
	}
}
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

	// C:\Library\Nuget
	// dotnet pack -c Release -o %cd%
	// dotnet nuget push "*.nupkg"
	// del *.nupkg & dotnet pack -c Release -o %cd%

	public unsafe static class Program
	{
		private static ProcessModule get(string s)
		{
			foreach (ProcessModule module in Process.GetCurrentProcess().Modules) {
				if (module.ModuleName==s) {
					return module;
				}
			}

			return null;
		}

		
		private static void Main(string[] args)
		{
			// E8 7C F8 FF FF
			Console.WriteLine("Hello");
			

		

			
		}


		private static int Inlined(string v)
		{
			return int.Parse(v);
		}

		private static int NotInlined(string v)
		{
			try
			{
				return int.Parse(v);
			}
			catch (Exception)
			{
				return 0;
			}
		}
	}
}
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
			public void Hi()
			{
				
			}

			public int Value {
				get { return 1; }
			}
		}
		
		static PropertyInfo GetProperty(MethodInfo method)
		{
			bool takesArg  = method.GetParameters().Length == 1;
			bool hasReturn = method.ReturnType != typeof(void);
			if (takesArg == hasReturn) return null;
			if (takesArg)
			{
				return method.DeclaringType
				             .GetProperties().FirstOrDefault(prop => prop.GetSetMethod() == method);
			}
			else
			{
				return method.DeclaringType
				             .GetProperties().FirstOrDefault(prop => prop.GetGetMethod() == method);
			}
		}
		
		private static void Main(string[] args)
		{
			Resources.SetupAll();
			

		}
	}
}
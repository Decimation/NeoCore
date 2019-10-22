#region

using System;
using System.Diagnostics;
using NeoCore.CoreClr.Framework;

#endregion

namespace NeoCore.CoreClr
{
	public static partial class Runtime
	{
		/// <summary>
		/// Contains utilities for retrieving information about the .NET runtime and its components.
		/// </summary>
		public static class Info
		{
			public static bool IsInDebugMode => Debugger.IsAttached;

			public static FrameworkIdentifier CurrentFramework {
				get {
					FrameworkTypes fwk = default;
					Version        vsn = null;

#if NETCOREAPP
					fwk = FrameworkTypes.Core;

#if NETCOREAPP3_0
					vsn = new Version(3, 0);
#endif

#endif

#if NETFRAMEWORK
					fwk = NetFrameworks.Framework;
#endif

#if NETSTANDARD
					fwk = NetFrameworks.Standard;
#endif

					return new FrameworkIdentifier(fwk, vsn);
				}
			}
		}
	}
}
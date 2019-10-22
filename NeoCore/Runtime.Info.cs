using System;

namespace NeoCore
{
	public static partial class Runtime
	{
		public static class Info
		{
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
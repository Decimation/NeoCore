using System;
using System.Diagnostics;
using NeoCore.CoreClr;

namespace NeoCore.Assets
{
	public sealed class ClrRuntimeAsset : RuntimeImportAsset
	{
		/// <summary>
		/// The framework type.
		/// </summary>
		public ClrFramework Framework { get; }

		public ClrRuntimeAsset(ClrFramework framework) : base(framework.LibraryFile, framework.SymbolFile)
		{
			// .NET Framework
			// Version: 4.0.30319.42000
			// symchk "C:\Windows\Microsoft.NET\Framework\v4.0.30319\clr.dll" /s SRV*C:\Users\Deci\Desktop\clr.pdb*http://msdl.microsoft.com/download/symbols
			// symchk "C:\Windows\Microsoft.NET\Framework64\v4.0.30319\clr.dll" /s SRV*C:\Users\Deci\Desktop\clr.pdb*http://msdl.microsoft.com/download/symbols
			
			// .NET Core
			// symchk "C:\Program Files\dotnet\shared\Microsoft.NETCore.App\3.0.0\coreclr.dll" /s SRV*C:\Users\Deci\Desktop\clr.pdb*http://msdl.microsoft.com/download/symbols

			// Version = new Version(4, 0, 30319, 42000);
			
			Framework = framework;
		}
	}
}
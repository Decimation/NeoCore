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
			// See spreadsheet

			// Version = new Version(4, 0, 30319, 42000);
			
			Framework = framework;
		}
	}
}
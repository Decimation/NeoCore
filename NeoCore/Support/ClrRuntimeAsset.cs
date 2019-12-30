using System;

namespace NeoCore.Support
{
	public sealed class ClrRuntimeAsset : RuntimeImportAsset
	{
		const string idx = @"C:\Users\Deci\RiderProjects\NeoCore\NeoCore\clr_image.txt";

		public ClrRuntimeAsset(ClrFramework framework) 
			: base(framework.LibraryFile, framework.SymbolFile, idx)
		{
			// See spreadsheet

			// Version = new Version(4, 0, 30319, 42000);
			
			
			Framework = framework;
		}

		/// <summary>
		/// The framework type.
		/// </summary>
		public ClrFramework Framework { get; }
	}
}
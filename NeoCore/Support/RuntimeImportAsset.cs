using System.IO;
using NeoCore.Import;
using NeoCore.Import.Providers.Symbol;

namespace NeoCore.Support
{
	public class RuntimeImportAsset : RuntimeAsset
	{
		/// <summary>
		/// Asset symbol access
		/// </summary>
		public ImportProvider Imports { get; }

		public RuntimeImportAsset(FileInfo libFile, FileInfo symFile) : base(libFile, symFile)
		{
			Imports = new SymbolImport(SymbolsFile, Module.BaseAddress);
		}
	}
}
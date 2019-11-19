using System.Diagnostics;
using System.IO;
using NeoCore.Import;

namespace NeoCore.Assets
{
	public class RuntimeImportAsset : RuntimeAsset
	{
		/// <summary>
		/// Asset symbol access
		/// </summary>
		public IImportProvider Imports { get; }

		public RuntimeImportAsset(FileInfo libFile, FileInfo symFile) : base(libFile, symFile)
		{
			Imports = new ModuleImport(SymbolsFile, Module);
		}
	}
}
using System.IO;
using NeoCore.Import;
using NeoCore.Import.Providers;

namespace NeoCore.Support
{
	public class RuntimeImportAsset : RuntimeAsset
	{
		public RuntimeImportAsset(FileInfo libFile, FileInfo symFile, string imgRecordFile) : base(libFile, symFile)
		{
			Imports = new ImageRecordImport(imgRecordFile, Module);
		}

		/// <summary>
		/// Asset symbol access
		/// </summary>
		public ImportProvider Imports { get; }
	}
}
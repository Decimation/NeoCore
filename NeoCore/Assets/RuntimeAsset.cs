using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using NeoCore.Import;
using NeoCore.Utilities;

namespace NeoCore.Assets
{
	public class RuntimeAsset
	{
		/// <summary>
		///     Asset DLL file
		/// </summary>
		public FileInfo LibraryFile { get; }

		/// <summary>
		///     Asset symbol file
		/// </summary>
		public FileInfo SymbolsFile { get; }

		/// <summary>
		///     The <see cref="ProcessModule" /> of this asset
		/// </summary>
		public ProcessModule Module { get; }

		/// <summary>
		/// Asset symbol access
		/// </summary>
		public IImportProvider Imports { get; }

		public RuntimeAsset(FileInfo dllFile, FileInfo symFile)
		{
			LibraryFile = dllFile;
			SymbolsFile = symFile;
			Module      = Modules.GetModule(dllFile.Name);
			Imports     = new ModuleImportProvider(SymbolsFile, Module);
		}
		
		public override string ToString()
		{
			var sb = new StringBuilder();

			sb.AppendFormat("Library: {0}\n", LibraryFile);
			sb.AppendFormat("Symbol: {0}\n", SymbolsFile);
			sb.AppendFormat("Module: {0}\n", Module.FileName);
			sb.AppendFormat("Imports: {0}\n", ((ModuleImportProvider) Imports).Address);

			return sb.ToString();
		}
	}
}
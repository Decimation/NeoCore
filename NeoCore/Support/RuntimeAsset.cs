using System.Diagnostics;
using System.IO;
using System.Text;
using Memkit;
using Memkit.Pointers;

namespace NeoCore.Support
{
	public class RuntimeAsset
	{
		public RuntimeAsset(FileInfo libFile, FileInfo symFile)
		{
			LibraryFile = libFile;
			SymbolsFile = symFile;
			Module      = Resources.CurrentProcess[libFile.Name];
		}

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

		public Pointer<byte> FindExport(string name)
		{
			return Memkit.Interop.Native.GetProcAddress(Module.BaseAddress, name);
		}

		public override string ToString()
		{
			var sb = new StringBuilder();

			sb.AppendFormat("Library: {0}\n", LibraryFile);
			sb.AppendFormat("Symbol: {0}\n", SymbolsFile);
			sb.AppendFormat("Module: {0}\n", Module.FileName);
			//sb.AppendFormat("Imports: {0}\n", ((ModuleImport) Imports).Address);

			return sb.ToString();
		}
	}
}
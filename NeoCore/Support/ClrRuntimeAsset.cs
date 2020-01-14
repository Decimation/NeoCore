using System;
using System.IO;
using System.Net;
using Memkit.Interop;
using NeoCore.Import.Providers;
using NeoCore.Win32;

namespace NeoCore.Support
{
	public sealed class ClrRuntimeAsset : RuntimeImportAsset
	{
		public ClrRuntimeAsset(ClrFramework framework) : this(framework, SearchForImageRecord()) { }

		public ClrRuntimeAsset(ClrFramework framework, string idx)
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

		private static string SearchForImageRecord()
		{
			string? path = Environment.GetEnvironmentVariable("PATH");
			string  cd   = Environment.CurrentDirectory;

			bool inPath = FileSystem.TryGetFullPath(ImageRecordImport.FILENAME, path, out string s);
			bool inCd   = FileSystem.TryGetFullPath(ImageRecordImport.FILENAME, cd, out s);

			if (s == null || !inPath || !inCd) {
				throw new FileNotFoundException();
			}

			return s;
		}
	}
}
using System;
using System.IO;
using NeoCore.Utilities;

namespace NeoCore.CoreClr.Support
{
	/// <summary>
	/// Contains possible CLR framework types.
	/// </summary>
	public static class ClrFrameworks
	{
		// https://docs.microsoft.com/en-us/dotnet/standard/frameworks
		// https://docs.microsoft.com/en-us/dotnet/standard/clr

		/// <summary>
		///     .NET Standard
		/// </summary>
		public static readonly ClrFramework Standard = new ClrFramework(nameof(Standard), null);


		/// <summary>
		///     .NET Core
		/// </summary>
		public static readonly ClrFramework Core = new ClrFramework(nameof(Core), "coreclr");

		/// <summary>
		///     .NET Framework
		/// </summary>
		public static readonly ClrFramework Framework = new ClrFramework(nameof(Framework), "clr");
	}

	/// <summary>
	/// Represents a CLR framework type.
	/// </summary>
	public readonly struct ClrFramework
	{
		private readonly FileInfo m_symbolFile;
		private readonly FileInfo m_libraryFile;

		public string Name { get; }

		public FileInfo SymbolFile => m_symbolFile;

		public FileInfo LibraryFile => m_libraryFile;

		public bool IsValid { get; }

		public ClrFramework(string name, string filenameStub)
		{
			Name = name;

			const string PDB_EXT = ".pdb";
			const string DLL_EXT = ".dll";

			FileSystem.TryGetRuntimeFile(filenameStub + DLL_EXT, out m_libraryFile);
			FileSystem.TryGetRuntimeFile(filenameStub + PDB_EXT, out m_symbolFile);
			
			IsValid = (m_symbolFile != null && m_symbolFile.Exists)
			          && (m_libraryFile != null && m_libraryFile.Exists)
			          && filenameStub != null;
		}

		public override string ToString()
		{
			return String.Format(".NET {0}", Name);
		}
	}
}
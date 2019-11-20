using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using NeoCore.CoreClr;
using NeoCore.Interop;
using NeoCore.Utilities;

namespace NeoCore.Assets
{
	/// <summary>
	/// Contains possible CLR framework types.
	/// </summary>
	public static class ClrFrameworks
	{
		// https://docs.microsoft.com/en-us/dotnet/standard/frameworks
		// https://docs.microsoft.com/en-us/dotnet/standard/clr


		/// <summary>
		///     .NET Core
		/// </summary>
		public static readonly ClrFramework Core = new ClrFramework(nameof(Core))
		{
			FilenameRoot     = "coreclr",
			PreprocessorName = "NETCOREAPP"
		};

		/// <summary>
		///     .NET Framework
		/// </summary>
		public static readonly ClrFramework Framework = new ClrFramework(nameof(Framework))
		{
			FilenameRoot     = "clr",
			PreprocessorName = "NETFRAMEWORK"
		};

		/// <summary>
		///     .NET Native
		/// </summary>
		public static readonly ClrFramework Native = new ClrFramework(nameof(Native));

		/// <summary>
		///     .NET Standard
		/// </summary>
		public static readonly ClrFramework Standard = new ClrFramework(nameof(Standard))
		{
			PreprocessorName = "NETSTANDARD"
		};

		/// <summary>
		///     Mono
		/// </summary>
		public static readonly ClrFramework Mono = new ClrFramework(nameof(Mono));

		public static readonly ClrFramework[] AllFrameworks = {Core, Framework, Native, Standard, Mono};
	}

	/// <summary>
	/// Represents a CLR framework type.
	/// </summary>
	public struct ClrFramework
	{
		public FileInfo SymbolFile {
			get {
				
				return Runtime.GetRuntimeFile(FilenameRoot + SystemOS.PDB_EXT);
			}
		}

		public FileInfo LibraryFile {
			get {
				
				return Runtime.GetRuntimeFile(FilenameRoot + SystemOS.DLL_EXT);
			}
		}

		public string Name { get; }

		public string FullName { get; }

		public string PreprocessorName { get; internal set; }

		public string FilenameRoot { get; internal set; }

		public ClrFramework([NotNull] string name)
		{
			Name     = name;
			FullName = String.Format(".NET {0}", Name);

			PreprocessorName = null;
			FilenameRoot     = null;
		}

		private bool Equals(ClrFramework other) => Name == other.Name;

		public override bool Equals(object obj) =>
			ReferenceEquals(this, obj) || obj is ClrFramework other && Equals(other);

		public override int GetHashCode() => (Name != null ? Name.GetHashCode() : 0);

		public static bool operator ==(ClrFramework left, ClrFramework right) => Equals(left, right);

		public static bool operator !=(ClrFramework left, ClrFramework right) => !Equals(left, right);

		public override string ToString() => FullName;
	}
}
using System.Diagnostics;
using System.IO;
using NeoCore.Memory.Pointers;

namespace NeoCore.Import.Providers
{
	/// <summary>
	/// Matches imports against memory
	/// </summary>
	public sealed class SymbolImport : ImportProvider
	{
		public SymbolImport(FileInfo pdb, ProcessModule module) : base(module)
		{
			SymbolFile = pdb;
		}

		public FileInfo SymbolFile { get; }

		private Interop.Structures.Symbol GetSymbol(string name)
		{
			SymbolManager.Value.CurrentImage = SymbolFile;
			return SymbolManager.Value.GetSymbol(name);
		}

		public override Pointer<byte> GetAddress(string id)
		{
			long ofs = GetSymbol(id).Offset;
			return BaseAddress + ofs;
		}

		public override Pointer<byte>[] GetAddresses(string[] ids)
		{
			SymbolManager.Value.CurrentImage = SymbolFile;
			long[] offsets = SymbolManager.Value.GetSymOffsets(ids);

			var rg = new Pointer<byte>[offsets.Length];

			for (int i = 0; i < rg.Length; i++) {
				rg[i] = BaseAddress + offsets[i];
			}

			return rg;
		}
	}
}
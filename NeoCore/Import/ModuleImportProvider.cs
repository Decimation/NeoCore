using System;
using System.Diagnostics;
using System.IO;
using NeoCore.Interop.Structures;
using NeoCore.Memory;
using NeoCore.Utilities.Diagnostics;

namespace NeoCore.Import
{
	/// <summary>
	/// Combines a process module with a PDB file, allowing access to the module's symbols.
	/// </summary>
	internal class ModuleImportProvider : IImportProvider
	{
		public FileInfo      SymbolFile { get; }
		public Pointer<byte> Address    { get; }

		internal ModuleImportProvider(FileInfo pdb, ProcessModule module) : this(pdb, module.BaseAddress) { }

		private ModuleImportProvider(FileInfo pdb, Pointer<byte> baseAddr)
		{
			Guard.AssertNotNull(baseAddr, nameof(baseAddr));

			Address    = baseAddr;
			SymbolFile = pdb;
		}

		private Symbol GetSymbol(string name)
		{
			SymbolManager.Value.CurrentImage = SymbolFile;
			return SymbolManager.Value.GetSymbol(name);
		}

		public Pointer<byte> GetAddress(string id)
		{
			long ofs = GetSymbol(id).Offset;
			return Address + ofs;
		}

		public Pointer<byte>[] GetAddresses(string[] names)
		{
			SymbolManager.Value.CurrentImage = SymbolFile;
			var offsets = SymbolManager.Value.GetSymOffsets(names);

			var rg = new Pointer<byte>[offsets.Length];

			for (int i = 0; i < rg.Length; i++) {
				rg[i] = Address + offsets[i];
			}

			return rg;
		}

		public TDelegate GetFunctionSafe<TDelegate>(string name) where TDelegate : Delegate
		{
			//return FunctionFactory.Delegates.CreateSafe<TDelegate>(GetAddress(name));
			throw new NotImplementedException();
		}

		public TDelegate GetFunction<TDelegate>(string id) where TDelegate : Delegate
		{
			//return FunctionFactory.Delegates.Create<TDelegate>(GetAddress(id));
			throw new NotImplementedException();
		}
	}
}
using System;
using System.Diagnostics;
using System.IO;
using NeoCore.Interop.Structures;
using NeoCore.Memory;
using NeoCore.Memory.Pointers;
using NeoCore.Utilities.Diagnostics;

namespace NeoCore.Import
{
	/// <summary>
	/// Combines a process module with a PDB file, allowing access to the module's symbols.
	/// </summary>
	internal class ModuleImport : IImportProvider
	{
		public FileInfo SymbolFile { get; }

		public Pointer<byte> Address { get; }

		internal ModuleImport(FileInfo pdb, ProcessModule module) : this(pdb, module.BaseAddress) { }

		private ModuleImport(FileInfo pdb, Pointer<byte> baseAddr)
		{
			Guard.AssertNotNull(baseAddr, nameof(baseAddr));

			Address    = baseAddr;
			SymbolFile = pdb;
		}

		internal Symbol GetSymbol(string name)
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
			long[] offsets = SymbolManager.Value.GetSymOffsets(names);

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
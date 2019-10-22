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
		private readonly FileInfo      m_pdb;
		private readonly Pointer<byte> m_baseAddress;

		internal ModuleImportProvider(FileInfo pdb, ProcessModule module) : this(pdb, module.BaseAddress) { }

		private ModuleImportProvider(FileInfo pdb, Pointer<byte> baseAddr)
		{
			Guard.NullCheck(baseAddr, nameof(baseAddr));

			m_baseAddress = baseAddr;
			m_pdb         = pdb;
		}

		private Symbol GetSymbol(string name)
		{
			SymbolManager.Value.CurrentImage = m_pdb;
			return SymbolManager.Value.GetSymbol(name);
		}

		public Pointer<byte> GetAddress(string id)
		{
			long ofs = GetSymbol(id).Offset;
			return m_baseAddress + ofs;
		}

		public Pointer<byte>[] GetAddresses(string[] names)
		{
			SymbolManager.Value.CurrentImage = m_pdb;
			var offsets = SymbolManager.Value.GetSymOffsets(names);

			var rg = new Pointer<byte>[offsets.Length];

			for (int i = 0; i < rg.Length; i++) {
				rg[i] = m_baseAddress + offsets[i];
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
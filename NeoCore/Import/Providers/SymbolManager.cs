using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using NeoCore.Interop;
using NeoCore.Interop.Structures;
using NeoCore.Model;
using NeoCore.Utilities.Diagnostics;

// ReSharper disable ReturnTypeCanBeEnumerable.Global
// ReSharper disable RedundantAssignment

namespace NeoCore.Import.Providers
{
	/// <summary>
	/// Provides access to symbols in a specified image
	/// <para>https://github.com/Microsoft/microsoft-pdb</para>
	/// </summary>
	internal sealed class SymbolManager : Releasable
	{
		private ulong        m_modBase;
		private FileInfo?    m_pdb;

		private IntPtr       m_proc;
		private string       m_singleNameBuffer;
		private List<Interop.Structures.Symbol> m_symBuffer;
		protected override string Id => nameof(SymbolManager);

		internal bool IsImageLoaded {
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => m_modBase != default;
		}

		internal FileInfo CurrentImage {
			get => m_pdb;
			set {
				if (m_pdb != value) {
					m_pdb = value;
					Load();
				}
			}
		}

		public override void Close()
		{
			UnloadModule();
			Native.Symbols.Cleanup(m_proc);

			m_proc    = IntPtr.Zero;
			m_modBase = default;
			m_pdb     = null;

			ClearBuffer();

			// Delete instance
			Value = null;

			base.Close();
		}

		public override void Setup()
		{
			m_proc = Native.Kernel.GetCurrentProcess();

			var options = Native.Symbols.GetOptions();

			// SYMOPT_DEBUG option asks DbgHelp to print additional troubleshooting
			// messages to debug output - use the debugger's Debug Output window
			// to view the messages

			options |= SymbolOptions.DEBUG;

			Native.Symbols.SetOptions(options);

			// Initialize DbgHelp and load symbols for all modules of the current process 
			Native.Symbols.Initialize(m_proc);


			base.Setup();
		}

		private void UnloadModule()
		{
			if (IsImageLoaded) {
				Native.Symbols.UnloadModule64(m_proc, m_modBase);
			}
		}

		private void CheckModule()
		{
			if (!IsImageLoaded) {
				Guard.Fail("This may be an error with loading. Have you loaded an image?");
			}
		}

		private void Load()
		{
			string img = m_pdb.FullName;

			Global.Value.WriteVerbose(Id, "Loading image {Img}", m_pdb.Name);

			UnloadModule();

			Guard.Assert(IsSetup);

			Native.FileSystem.GetFileParams(img, out ulong baseAddr, out ulong fileSize);

			m_modBase = Native.Symbols.LoadModuleEx(m_proc, img, baseAddr, (uint) fileSize);

			CheckModule();
		}

		internal long[] GetSymOffsets(string[] names) => GetSymbols(names).Select(x => x.Offset).ToArray();

		internal long GetSymOffset(string name) => GetSymbol(name).Offset;

		internal Interop.Structures.Symbol[] GetSymbols()
		{
			CheckModule();

			m_symBuffer = new List<Interop.Structures.Symbol>();

			Native.Symbols.EnumSymbols(m_proc, m_modBase, AddSymCallback);

			Interop.Structures.Symbol[] cpy = m_symBuffer.ToArray();
			ClearBuffer();

			return cpy;
		}

		internal Interop.Structures.Symbol[] GetSymbols(string[] names)
		{
			CheckModule();

			var rg = new Interop.Structures.Symbol[names.Length];

			for (int i = 0; i < rg.Length; i++) {
				rg[i] = GetSymbol(names[i]);
			}

			return rg;
		}

		// note: doesn't check module
		internal Interop.Structures.Symbol GetSymbol(string name) => Native.Symbols.GetSymbol(m_proc, name);

		private void ClearBuffer()
		{
			m_symBuffer?.Clear();

			m_singleNameBuffer = null;
			m_symBuffer        = null;
		}

		internal Interop.Structures.Symbol[] GetSymbolsContainingName(string name)
		{
			CheckModule();

			m_symBuffer        = new List<Interop.Structures.Symbol>();
			m_singleNameBuffer = name;

			Native.Symbols.EnumSymbols(m_proc, m_modBase, AddSymByNameCallback);

			Interop.Structures.Symbol[] cpy = m_symBuffer.ToArray();

			ClearBuffer();

			return cpy;
		}

		#region Singleton

		private SymbolManager()
		{
			Setup();
		}

		/// <summary>
		/// Gets an instance of <see cref="SymbolManager"/>
		/// </summary>
		internal static SymbolManager Value { get; private set; } = new SymbolManager();

		#endregion

		#region Callbacks

		private unsafe bool AddSymByNameCallback(IntPtr sym, uint symSize, IntPtr userCtx)
		{
			string symName = ((DebugSymbol*) sym)->ReadSymbolName();

			if (symName.Contains(m_singleNameBuffer)) {
				m_symBuffer.Add(new Interop.Structures.Symbol(sym));
			}

			return true;
		}

		private bool AddSymCallback(IntPtr sym, uint symSize, IntPtr userCtx)
		{
			m_symBuffer.Add(new Interop.Structures.Symbol(sym));

			return true;
		}

		#endregion
	}
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Memkit.Interop;
using NeoCore.Model;
using NeoCore.Utilities.Diagnostics;
using NeoCore.Win32.Structures;

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
		private List<Symbol> m_symBuffer;
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
			Win32.Native.Symbols.Cleanup(m_proc);

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
			m_proc = Native.GetCurrentProcess().Address;

			var options = Win32.Native.Symbols.GetOptions();

			// SYMOPT_DEBUG option asks DbgHelp to print additional troubleshooting
			// messages to debug output - use the debugger's Debug Output window
			// to view the messages

			options |= SymbolOptions.DEBUG;

			global::NeoCore.Win32.Native.Symbols.SetOptions(options);

			// Initialize DbgHelp and load symbols for all modules of the current process 
			Win32.Native.Symbols.Initialize(m_proc);


			base.Setup();
		}

		private void UnloadModule()
		{
			if (IsImageLoaded) {
				Win32.Native.Symbols.UnloadModule64(m_proc, m_modBase);
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

			Win32.Native.FileSystem.GetFileParams(img, out ulong baseAddr, out ulong fileSize);

			m_modBase = Win32.Native.Symbols.LoadModuleEx(m_proc, img, baseAddr, (uint) fileSize);

			CheckModule();
		}

		internal long[] GetSymOffsets(string[] names) => GetSymbols(names).Select(x => x.Offset).ToArray();

		internal long GetSymOffset(string name) => GetSymbol(name).Offset;

		internal Symbol[] GetSymbols()
		{
			CheckModule();

			m_symBuffer = new List<Symbol>();

			Win32.Native.Symbols.EnumSymbols(m_proc, m_modBase, AddSymCallback);

			Symbol[] cpy = m_symBuffer.ToArray();
			ClearBuffer();

			return cpy;
		}

		internal Symbol[] GetSymbols(string[] names)
		{
			CheckModule();

			var rg = new Symbol[names.Length];

			for (int i = 0; i < rg.Length; i++) {
				rg[i] = GetSymbol(names[i]);
			}

			return rg;
		}

		// note: doesn't check module
		internal Symbol GetSymbol(string name) => Win32.Native.Symbols.GetSymbol(m_proc, name);

		private void ClearBuffer()
		{
			m_symBuffer?.Clear();

			m_singleNameBuffer = null;
			m_symBuffer        = null;
		}

		internal Symbol[] GetSymbolsContainingName(string name)
		{
			CheckModule();

			m_symBuffer        = new List<Symbol>();
			m_singleNameBuffer = name;

			Win32.Native.Symbols.EnumSymbols(m_proc, m_modBase, AddSymByNameCallback);

			Symbol[] cpy = m_symBuffer.ToArray();

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
				m_symBuffer.Add(new Symbol(sym));
			}

			return true;
		}

		private bool AddSymCallback(IntPtr sym, uint symSize, IntPtr userCtx)
		{
			m_symBuffer.Add(new Symbol(sym));

			return true;
		}

		#endregion
	}
}
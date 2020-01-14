using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using NeoCore.Model;
using NeoCore.Support;
using NeoCore.Utilities.Diagnostics;
using NeoCore.Win32;
using NeoCore.Win32.Structures;
using Native = Memkit.Interop.Native;

// ReSharper disable UnusedMember.Global
// ReSharper disable ReturnTypeCanBeEnumerable.Global


namespace NeoCore.Import.Providers
{
	/// <summary>
	/// Provides access to symbols in a specified image
	/// <para>https://github.com/Microsoft/microsoft-pdb</para>
	/// </summary>
	internal sealed class SymbolManager : Releasable
	{
		private ulong     m_modBase;
		private FileInfo? m_pdb;

		private            IntPtr       m_proc;
		private            string       m_singleNameBuffer;
		private            List<Symbol> m_symBuffer;
		protected override string       Id => nameof(SymbolManager);

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
			Symbols.Cleanup(m_proc);

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

			var options = Symbols.GetOptions();

			// SYMOPT_DEBUG option asks DbgHelp to print additional troubleshooting
			// messages to debug output - use the debugger's Debug Output window
			// to view the messages

			options |= SymbolOptions.DEBUG;

			Symbols.SetOptions(options);

			// Initialize DbgHelp and load symbols for all modules of the current process 
			Symbols.Initialize(m_proc);


			base.Setup();
		}

		private void UnloadModule()
		{
			if (IsImageLoaded) {
				Symbols.UnloadModule64(m_proc, m_modBase);
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

			// Determine its size, and use a dummy base address 

			// it can be any non-zero value, but if we load symbols 
			// from more than one file, memory regions specified
			// for different files should not overlap
			// (region is "base address + file size")
			const ulong DLL_BASE_ADDR = 0x10000000;

			var hFile = FileSystem.CreateFile(img, FileAccess.Read, FileShare.Read,
			                                  FileMode.Open, default);

			var fileSize = FileSystem.GetFileSize(hFile);

			Native.CloseHandle(hFile);

			m_modBase = Symbols.LoadModuleEx(m_proc, img, DLL_BASE_ADDR, fileSize);

			CheckModule();
		}

		internal long[] GetSymOffsets(string[] names) => GetSymbols(names).Select(x => x.Offset).ToArray();

		internal long GetSymOffset(string name) => GetSymbol(name).Offset;

		internal Symbol[] GetSymbols()
		{
			CheckModule();

			m_symBuffer = new List<Symbol>();

			Symbols.EnumSymbols(m_proc, m_modBase, AddSymCallback);

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
		internal Symbol GetSymbol(string name) => Symbols.GetSymbol(m_proc, name);

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

			Symbols.EnumSymbols(m_proc, m_modBase, AddSymByNameCallback);

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
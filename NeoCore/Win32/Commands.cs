using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Memkit.Interop;

// ReSharper disable ReturnTypeCanBeEnumerable.Global

// ReSharper disable InconsistentNaming

namespace NeoCore.Win32
{
	/// <summary>
	/// Contains utilities for working with Windows CMD/CLI.
	/// </summary>
	public static class Commands
	{
		public static FileInfo RunSymCheck(FileInfo dll, DirectoryInfo output)
		{
			// symchk "dll" /s SRV*output.pdb*http://msdl.microsoft.com/download/symbols
			// symchk "[DLL_SRC]" /s SRV*[OUTPUT_FILENAME_FULL]*[SERVER]
			// symchk "[DLL_SRC]" /s SRV*[OUTPUT_FILENAME_FULL]*http://msdl.microsoft.com/download/symbols
			// symchk "C:\Program Files\dotnet\shared\Microsoft.NETCore.App\3.0.0\coreclr.dll" /s SRV*C:\Users\Deci\Desktop\\coreclr.pdb*http://msdl.microsoft.com/download/symbols

			// See spreadsheet

			const string MSFT_SYM_SERVER = "http://msdl.microsoft.com/download/symbols";

			string outputArg          = output.FullName;
			string outputPdbFile      = Path.GetFileNameWithoutExtension(dll.Name) + Native.PDB_EXT;
			string dllSrc             = dll.FullName;
			string outputFilenameFull = Path.Combine(outputArg, outputPdbFile);

			string fullCmd = String.Format("{0} \"{1}\" /s SRV*{2}*{3}", Native.SYMCHK_EXE, dllSrc, outputFilenameFull,
			                               MSFT_SYM_SERVER);


			string[] stdOut = ShellOutput(fullCmd);

			bool success = stdOut.Any(line => line.Contains("SYMCHK: PASSED + IGNORED files = 1"));

			if (!success) {
				throw new Win32Exception();
			}

			var outputRoot = new DirectoryInfo(Path.Combine(outputArg, outputPdbFile));

			return outputRoot.GetDirectories().First()
			                 .GetDirectories().First()
			                 .GetFiles().First();
		}

		public static string[] ShellOutput(string cmd)
		{
			var proc   = Shell(cmd, true);
			var stdOut = proc.StandardOutput;
			var list   = new List<string>();

			while (!stdOut.EndOfStream) {
				string line = stdOut.ReadLine();

				if (line != null) {
					list.Add(line);
				}
			}

			proc.WaitForExit();

			return list.ToArray();
		}

		/// <summary>
		/// Creates a <see cref="Process"/> to execute <paramref name="cmd"/>
		/// </summary>
		/// <param name="cmd">Command to run</param>
		/// <param name="autoStart">Whether to automatically start the <c>cmd.exe</c> process</param>
		/// <returns><c>cmd.exe</c> process</returns>
		public static Process Shell(string cmd, bool autoStart = false)
		{
			var startInfo = new ProcessStartInfo
			{
				FileName               = Native.CMD_EXE,
				Arguments              = String.Format("/C {0}", cmd),
				RedirectStandardOutput = true,
				RedirectStandardError  = true,
				UseShellExecute        = false,
				CreateNoWindow         = true
			};

			var process = new Process
			{
				StartInfo           = startInfo,
				EnableRaisingEvents = true
			};

			if (autoStart)
				process.Start();

			return process;
		}
	}
}
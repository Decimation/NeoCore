using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using NeoCore.Utilities;
using NeoCore.Utilities.Diagnostics;

// ReSharper disable ReturnTypeCanBeEnumerable.Global

// ReSharper disable InconsistentNaming

namespace NeoCore.Interop
{
	/// <summary>
	/// Contains utilities for working with Windows CMD/CLI.
	/// </summary>
	public static class Cli
	{
		public static FileInfo RunSymCheck(FileInfo dll, DirectoryInfo output)
		{
			// symchk "dll" /s SRV*output.pdb*http://msdl.microsoft.com/download/symbols
			
			// See spreadsheet
			
			const string MSFT_SYM_SERVER = "http://msdl.microsoft.com/download/symbols";

			string outputArg     = output.FullName;
			string outputPdbFile = Path.GetFileNameWithoutExtension(dll.Name) + Native.PDB_EXT;
			
			var cmdBuilder = new CommandBuilder(Native.SYMCHK_EXE, "{dll} /s SRV*{out}\\{pdb}*{srv}");

			string fullCmd = cmdBuilder.AddString("dll", dll.FullName)
			                           .Add("out", outputArg)
			                           .Add("pdb", outputPdbFile)
			                           .Add("srv", MSFT_SYM_SERVER)
			                           .ToString();


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
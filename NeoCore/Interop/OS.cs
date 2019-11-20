using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using NeoCore.Utilities;
using NeoCore.Utilities.Diagnostics;

namespace NeoCore.Interop
{
	public static class OS
	{
		public const string PDB_EXT = ".pdb";
		public const string DLL_EXT = ".dll";
		
		public static FileInfo RunSymSrv(FileInfo dll, DirectoryInfo output)
		{
			// .NET Framework
			// Version: 4.0.30319.42000
			// symchk "C:\Windows\Microsoft.NET\Framework\v4.0.30319\clr.dll" /s SRV*C:\Users\Deci\Desktop\clr.pdb*http://msdl.microsoft.com/download/symbols
			// symchk "C:\Windows\Microsoft.NET\Framework64\v4.0.30319\clr.dll" /s SRV*C:\Users\Deci\Desktop\clr.pdb*http://msdl.microsoft.com/download/symbols

			// .NET Core
			// symchk "C:\Program Files\dotnet\shared\Microsoft.NETCore.App\3.0.0\coreclr.dll" /s SRV*C:\Users\Deci\Desktop\clr.pdb*http://msdl.microsoft.com/download/symbols

			const string cmd       = "symchk";
			const string serverArg = "http://msdl.microsoft.com/download/symbols";

			string dllArg        = dll.FullName;
			string outputArg     = output.FullName;
			string outputPdbFile = Path.GetFileNameWithoutExtension(dll.Name) + PDB_EXT;

			string fullCmd = String.Format("{0} \"{1}\" /s SRV*{2}\\{3}*{4}", cmd, dllArg,
			                               outputArg, outputPdbFile, serverArg);

			string[] stdOut  = ShellOutput(fullCmd);
			bool     success = stdOut.Any(line => line.Contains("SYMCHK: PASSED + IGNORED files = 1"));

			if (!success) {
				throw new Win32Exception();
			}
			
			var outputFolder = new DirectoryInfo(outputPdbFile);

			Format.ListDirectory(outputFolder);

			return null;

			/*var subdirs = outputFolder.GetDirectories();
			var nextFolder = subdirs.First(s => s.Name == outputPdbFile);
			nextFolder = nextFolder.GetDirectories().First();
			var pdbFile = nextFolder.GetFiles().First();

			return pdbFile;*/
		}

		public static string[] ShellOutput(string cmd)
		{
			var proc   = Shell(cmd, true);
			var stdOut = proc.StandardOutput;
			var list   = new List<string>();

			while (!stdOut.EndOfStream) {
				var line = stdOut.ReadLine();

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
			const string CMD_EXE = "cmd.exe";

			var startInfo = new ProcessStartInfo
			{
				FileName               = CMD_EXE,
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
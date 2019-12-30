using System;
using System.Diagnostics;
using System.Text;

namespace NeoCore.Import.Providers
{
	public readonly struct ImageRecordInfo
	{
		public string  Module  { get; }
		public Version Version { get; }

		internal ImageRecordInfo(string module, Version v)
		{
			Module  = module;
			Version = v;
		}
		
		internal bool Check(ProcessModule module)
		{
			bool nameCheck = module.ModuleName == Module;

			var info = module.FileVersionInfo;

			var moduleVersion = new Version(info.FileMajorPart, info.FileMinorPart,
			                                info.FileBuildPart, info.FilePrivatePart);


			bool versionCheck = moduleVersion == Version;

			return nameCheck && versionCheck;
		}

		public override string ToString()
		{
			var sb = new StringBuilder();
			sb.AppendFormat("Module: {0}\n", Module);
			sb.AppendFormat("Version: {0}\n", Version);
			return sb.ToString();
		}
	}
}
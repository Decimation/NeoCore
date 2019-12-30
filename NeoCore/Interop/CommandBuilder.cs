using System;
using System.Text;
using NeoCore.Utilities;

namespace NeoCore.Interop
{
	/// <summary>
	/// Builds a command for Windows CMD/CLI.
	/// </summary>
	public sealed class CommandBuilder
	{
		private readonly StringBuilder m_cmdBuild;
		private readonly string m_cmdFmt;
		private readonly string m_exe;

		public CommandBuilder(string exe, string cmdFmt)
		{
			m_exe    = exe;
			m_cmdFmt = cmdFmt;

			m_cmdBuild = new StringBuilder();
			
			m_cmdBuild.Append(exe)
			          .Append(Format.SPACE)
			          .Append(cmdFmt);
		}

		public CommandBuilder Add(string value)
		{
			m_cmdBuild.Append(Format.SPACE)
			          .Append(value)
			          .Append(Format.SPACE);

			return this;
		}

		public CommandBuilder Add(string token, string v)
		{
			m_cmdBuild.Replace(String.Format("{{{0}}}", token), v);
			return this;
		}

		public CommandBuilder AddString(string token, string value)
		{
			Add(token, "\"" + value + "\"");
			return this;
		}

		public override string ToString()
		{
			return m_cmdBuild.ToString().TrimEnd(Format.SPACE);
		}
	}
}
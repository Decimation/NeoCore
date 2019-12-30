using System.Diagnostics;
using System.Linq;
using System.Text;

namespace NeoCore.Utilities
{
	public sealed class NeoProcess
	{
		public NeoProcess(Process proc)
		{
			Process = proc;
			Modules = Process.Modules.Cast<ProcessModule>().ToArray();
		}

		public Process Process { get; }

		public ProcessModule[] Modules { get; }

		/// <summary>
		/// Returns the first <see cref="ProcessModule"/> whose <see cref="ProcessModule.ModuleName"/> equals
		/// <paramref name="name"/>; <c>null</c> if no match was found.
		/// </summary>
		/// <param name="name"><see cref="ProcessModule.ModuleName"/></param>
		public ProcessModule this[string name] {
			get { return Modules.FirstOrDefault(m => m.ModuleName == name); }
		}

		public static implicit operator NeoProcess(Process proc) => new NeoProcess(proc);

		public override string ToString()
		{
			var sb = new StringBuilder();
			sb.AppendFormat("Process: {0}\n", Process.ProcessName);
			sb.AppendFormat("Modules: {0}\n", Modules.Length);
			return sb.ToString();
		}
	}
}
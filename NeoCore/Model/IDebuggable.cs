using NeoCore.Utilities;
using SimpleCore;
using SimpleCore.Formatting;

namespace NeoCore.Model
{
	public interface IDebuggable
	{
		ConsoleTable DebugTable { get; }
		string Debug => DebugTable.ToString();
	}
}
using NeoCore.Utilities;

namespace NeoCore.Model
{
	public interface IDebuggable
	{
		ConsoleTable DebugTable { get; }
		string Debug => DebugTable.ToString();
	}
}
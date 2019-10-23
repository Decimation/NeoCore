using NeoCore.Assets;
using NeoCore.Import;
using NeoCore.Import.Attributes;

namespace NeoCore.Interop
{
	/// <summary>
	/// Provides methods for creating and modifying functions.
	/// </summary>
	[ImportNamespace]
	public static unsafe partial class FunctionFactory
	{
		static FunctionFactory()
		{
			ImportManager.Value.Load(typeof(FunctionFactory), Resources.Clr.Imports);
		}

		[ImportMapField]
		private static readonly ImportMap Imports = new ImportMap();
	}
}
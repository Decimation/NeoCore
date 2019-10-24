using System.Runtime.InteropServices;
using NeoCore.Interop.Structures;

namespace NeoCore.Assets
{
	public static partial class Constants
	{
		internal static class Native
		{
			/// <summary>
			/// Max string length for <see cref="DebugSymbol.Name"/>
			/// </summary>
			internal const int MaxSymbolNameLength = 2000;

			/// <summary>
			/// Size of <see cref="DebugSymbol"/>
			/// </summary>
			internal static readonly int StructureSize = Marshal.SizeOf<DebugSymbol>();
		}
	}
}
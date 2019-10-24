using NeoCore.Memory;
using NeoCore.Memory.Pointers;

namespace NeoCore.Assets
{
	public static partial class Constants
	{
		internal static class Defaults
		{
			/// <summary>
			/// Default offset for <see cref="Pointer{T}"/>
			/// </summary>
			internal const int DEF_OFFSET = 0;

			/// <summary>
			/// Default increment/decrement/element count for <see cref="Pointer{T}"/>
			/// </summary>
			internal const int DEF_ELEM_CNT = 1;
		}
	}
}
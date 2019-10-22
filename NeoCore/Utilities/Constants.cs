using NeoCore.Memory;

namespace NeoCore.Utilities
{
	internal static class Constants
	{
		/// <summary>
		/// Default offset for <see cref="Pointer{T}"/>
		/// </summary>
		internal const int DEF_OFFSET = 0;

		/// <summary>
		/// Default increment/decrement/element count for <see cref="Pointer{T}"/>
		/// </summary>
		internal const int DEF_ELEM_CNT = 1;

		internal const int INVALID_VALUE = -1;
	}
}
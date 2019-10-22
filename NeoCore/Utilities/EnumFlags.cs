using System;
using NeoCore.Import;

namespace NeoCore.Utilities
{
	/// <summary>
	/// Contains optimized versions of the <see cref="Enum.HasFlag"/> function.
	/// </summary>
	public static class EnumFlags
	{
		// ((uThis & uFlag) == uFlag)

		
		public static bool HasFlagFast(this IdentifierOptions value, IdentifierOptions flag)
		{
			return (value & flag) == flag;
		}
		
		public static bool HasFlagFast(this ImportCallOptions value, ImportCallOptions flag)
		{
			return (value & flag) == flag;
		}
	}
}
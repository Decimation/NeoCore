using System;
using NeoCore.CoreClr.Metadata;
using NeoCore.Import;

namespace NeoCore.Utilities
{
	/// <summary>
	/// Contains optimized versions of the <see cref="Enum.HasFlag"/> function.
	/// </summary>
	public static class EnumFlags
	{
		// ((uThis & uFlag) == uFlag)
		
		public static bool HasFlagFast(this CodeInfo value, CodeInfo flag)
		{
			return (value & flag) != 0;
		}

		public static bool HasFlagFast(this LayoutFlags value, LayoutFlags flag)
		{
			return (value & flag) != 0;
		}

		public static bool HasFlagFast(this VMFlags value, VMFlags flag)
		{
			return (value & flag) != 0;
		}

		public static bool HasFlagFast(this TypeInfo value, TypeInfo flag)
		{
			return (value & flag) != 0;
		}

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
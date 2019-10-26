using System;
using NeoCore.CoreClr.Meta;
using NeoCore.CoreClr.VM;
using NeoCore.Import;

namespace NeoCore.Utilities.Extensions
{
	/// <summary>
	/// Contains optimized versions of the <see cref="Enum.HasFlag"/> function.
	/// </summary>
	public static class EnumExtensions
	{
		// ((uThis & uFlag) == uFlag)
		// ((uThis & uFlag) != 0)

		public static bool HasFlagFast(this AuxiliaryProperties value, AuxiliaryProperties flag) => (value & flag) != 0;

		public static bool HasFlagFast(this FieldProperties value, FieldProperties flag) => (value & flag) != 0;

		public static bool HasFlagFast(this CodeInfo value, CodeInfo flag) => (value & flag) != 0;

		public static bool HasFlagFast(this LayoutFlags value, LayoutFlags flag) => (value & flag) != 0;

		public static bool HasFlagFast(this VMFlags value, VMFlags flag) => (value & flag) != 0;

		public static bool HasFlagFast(this TypeInfo value, TypeInfo flag) => (value & flag) != 0;

		public static bool HasFlagFast(this IdentifierOptions value, IdentifierOptions flag) => (value & flag) != 0;

		public static bool HasFlagFast(this ImportCallOptions value, ImportCallOptions flag) => (value & flag) != 0;
	}
}
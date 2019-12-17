using System;
using NeoCore.CoreClr.Meta;
using NeoCore.CoreClr.VM;
using NeoCore.Import;
using NeoCore.Import.Attributes;
using NeoCore.Interop.Structures;

namespace NeoCore.Utilities.Extensions
{
	/// <summary>
	/// Contains optimized versions of the <see cref="Enum.HasFlag"/> function.
	/// </summary>
	public static class EnumExtensions
	{
		// ((uThis & uFlag) == uFlag)
		// ((uThis & uFlag) != 0)
		
		public static bool HasFlagFast(this ImageFileCharacteristics value, ImageFileCharacteristics flag) => (value & flag) != 0;
		
		public static bool HasFlagFast(this HexOptions value, HexOptions flag) => (value & flag) != 0;

		public static bool HasFlagFast(this AuxiliaryProperties value, AuxiliaryProperties flag) => (value & flag) != 0;

		public static bool HasFlagFast(this FieldBitFlags value, FieldBitFlags flag) => (value & flag) != 0;

		public static bool HasFlagFast(this CodeFlags value, CodeFlags flag) => (value & flag) != 0;

		public static bool HasFlagFast(this LayoutFlags value, LayoutFlags flag) => (value & flag) != 0;

		public static bool HasFlagFast(this VMFlags value, VMFlags flag) => (value & flag) != 0;

		public static bool HasFlagFast(this TypeFlags value, TypeFlags flag) => (value & flag) != 0;

		public static bool HasFlagFast(this IdentifierOptions value, IdentifierOptions flag) => (value & flag) != 0;

		public static bool HasFlagFast(this ImportCallOptions value, ImportCallOptions flag) => (value & flag) != 0;
	}
}
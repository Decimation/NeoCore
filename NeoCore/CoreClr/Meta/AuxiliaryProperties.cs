using System;

namespace NeoCore.CoreClr.Meta
{
	/// <summary>
	/// Additional type properties.
	/// </summary>
	[Flags]
	public enum AuxiliaryProperties
	{
		None = 0,
		
		Integer = 1,
		
		Real = 1 << 1,
		
		Struct = 1 << 2,
		
		Pointer = 1 << 3,
		
		Unmanaged = 1 << 4,
		
		Enumerable = 1 << 5,
		
		Numeric = Integer & Real
	}
}
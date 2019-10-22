using System;
using System.Runtime.InteropServices;
using NeoCore.Interop.Attributes;

namespace NeoCore.Interop.Structures
{
	#region

	#endregion

	[NativeStructure]
	[StructLayout(LayoutKind.Sequential)]
	internal struct SymbolInfo
	{
		internal uint  SizeOfStruct;
		internal uint  TypeIndex;
		internal ulong Reserved_1;
		internal ulong Reserved_2;
		internal uint  Index;
		internal uint  Size;
		internal ulong ModBase;
		internal uint  Flags;
		internal ulong Value;
		internal ulong Address;
		internal uint  Register;
		internal uint  Scope;
		internal uint  Tag;
		internal uint  NameLen;
		internal uint  MaxNameLen;
		internal sbyte Name;
	}
}
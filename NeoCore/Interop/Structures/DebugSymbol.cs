using System;
using System.Runtime.InteropServices;
using NeoCore.Interop.Attributes;
using NeoCore.Interop.Enums;

namespace NeoCore.Interop.Structures
{
	#region

	#endregion

	[NativeStructure]
	[StructLayout(LayoutKind.Sequential)]
	internal struct DebugSymbol
	{
		// https://docs.microsoft.com/en-us/windows/win32/api/dbghelp/ns-dbghelp-symbol_info
		
		internal uint       SizeOfStruct;
		internal uint       TypeIndex;
		internal ulong      Reserved_1;
		internal ulong      Reserved_2;
		internal uint       Index;
		internal uint       Size;
		internal ulong      ModBase;
		internal SymbolFlag Flags;
		internal ulong      Value;
		internal ulong      Address;
		internal uint       Register;
		internal uint       Scope;
		internal SymbolTag  Tag;
		internal uint       NameLen;
		internal uint       MaxNameLen;
		internal sbyte      Name;
	}
}
using System;
using NeoCore.Interop.Enums;

namespace NeoCore.Interop.Structures
{
	/// <summary>
	///     Wraps a <see cref="DebugSymbol" />
	/// </summary>
	public unsafe class Symbol
	{
		internal Symbol(DebugSymbol* pSymInfo, string name)
		{
			Name = name;

			SizeOfStruct = pSymInfo->SizeOfStruct;
			TypeIndex    = pSymInfo->TypeIndex;
			Index        = pSymInfo->Index;
			Size         = pSymInfo->Size;
			ModBase      = pSymInfo->ModBase;
			Flags        = pSymInfo->Flags;
			Value        = pSymInfo->Value;
			Address      = pSymInfo->Address;
			Register     = pSymInfo->Register;
			Scope        = pSymInfo->Scope;
			Tag          = pSymInfo->Tag;
		}

		internal Symbol(IntPtr pSym) : this((DebugSymbol*) pSym, Native.DebugHelp.GetSymbolName(pSym)) { }


		public string Name { get; }

		public uint SizeOfStruct { get; }

		public uint TypeIndex { get; }

		public uint Index { get; }

		public uint Size { get; }

		public ulong ModBase { get; }

		public ulong Value { get; }

		public ulong Address { get; }

		public uint Register { get; }

		public uint Scope { get; }

		public SymbolTag Tag { get; }

		public SymbolFlag Flags { get; }

		public long Offset => (long) (Address - ModBase);


		private static int GetSymbolInfoSize(DebugSymbol* pSym)
		{
			// SizeOfStruct + (MaxNameLen - 1) * sizeof(TCHAR)
			return (int) (pSym->SizeOfStruct + (pSym->MaxNameLen - 1) * sizeof(byte));
		}

		public override string ToString()
		{
			return String.Format("Name: {0} | Offset: {1:X} | Address: {2:X} | Tag: {3} | Flags: {4}", Name, Offset,
			                     Address, Tag, Flags);
		}
	}
}
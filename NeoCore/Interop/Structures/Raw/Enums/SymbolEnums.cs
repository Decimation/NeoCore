using System;

// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming

namespace NeoCore.Interop.Structures.Raw.Enums
{
	[Flags]
	public enum SymbolFlag
	{
		/// <summary>
		/// The symbol is a CLR token.
		/// </summary>
		CLR_TOKEN = 0x00040000,


		/// <summary>
		/// The symbol is a constant.
		/// </summary>
		CONSTANT = 0x00000100,


		/// <summary>
		/// The symbol is from the export table.
		/// </summary>
		EXPORT = 0x00000200,


		/// <summary>
		/// The symbol is a forwarder.
		/// </summary>
		FORWARDER = 0x00000400,


		/// <summary>
		/// Offsets are frame relative.
		/// </summary>
		FRAMEREL = 0x00000020,


		/// <summary>
		/// The symbol is a known function.
		/// </summary>
		FUNCTION = 0x00000800,


		/// <summary>
		/// The symbol address is an offset relative to the beginning of the intermediate language block. This applies to managed code only.
		/// </summary>
		ILREL = 0x00010000,


		/// <summary>
		/// The symbol is a local variable.
		/// </summary>
		LOCAL = 0x00000080,


		/// <summary>
		/// The symbol is managed metadata.
		/// </summary>
		METADATA = 0x00020000,


		/// <summary>
		/// The symbol is a parameter.
		/// </summary>
		PARAMETER = 0x00000040,


		/// <summary>
		/// The symbol is a register. The Register member is used.
		/// </summary>
		REGISTER = 0x00000008,

		/// <summary>
		/// Offsets are register relative.
		/// </summary>
		REGREL = 0x00000010,

		/// <summary>
		/// The symbol is a managed code slot.
		/// </summary>
		SLOT = 0x00008000,

		/// <summary>
		/// The symbol is a thunk.
		/// </summary>
		THUNK = 0x00002000,

		/// <summary>
		/// The symbol is an offset into the TLS data area.
		/// </summary>
		TLSREL = 0x00004000,

		/// <summary>
		/// The Value member is used.
		/// </summary>
		VALUEPRESENT = 0x00000001,

		/// <summary>
		/// The symbol is a virtual symbol created by the SymAddSymbol function.
		/// </summary>
		VIRTUAL = 0x00001000,
	}

	[Flags]
	public enum SymbolOptions : uint
	{
		ALLOW_ABSOLUTE_SYMBOLS = 0x00000800,

		ALLOW_ZERO_ADDRESS = 0x01000000,

		AUTO_PUBLICS = 0x00010000,

		CASE_INSENSITIVE = 0x00000001,

		DEBUG = 0x80000000,

		DEFERRED_LOADS = 0x00000004,

		DISABLE_SYMSRV_AUTODETECT = 0x02000000,

		EXACT_SYMBOLS = 0x00000400,

		FAIL_CRITICAL_ERRORS = 0x00000200,

		FAVOR_COMPRESSED = 0x00800000,

		FLAT_DIRECTORY = 0x00400000,

		IGNORE_CVREC = 0x00000080,

		IGNORE_IMAGEDIR = 0x00200000,

		IGNORE_NT_SYMPATH = 0x00001000,

		INCLUDE_32BIT_MODULES = 0x00002000,

		LOAD_ANYTHING = 0x00000040,

		LOAD_LINES = 0x00000010,

		NO_CPP = 0x00000008,

		NO_IMAGE_SEARCH = 0x00020000,

		NO_PROMPTS = 0x00080000,

		NO_PUBLICS = 0x00008000,

		NO_UNQUALIFIED_LOADS = 0x00000100,

		OVERWRITE = 0x00100000,

		PUBLICS_ONLY = 0x00004000,

		SECURE = 0x00040000,

		UNDNAME = 0x00000002,
	}

	public enum SymbolTag
	{
		Null,
		Exe,
		Compiland,
		CompilandDetails,
		CompilandEnv,
		Function,
		Block,
		Data,
		Annotation,
		Label,
		PublicSymbol,
		UDT,
		Enum,
		FunctionType,
		PointerType,
		ArrayType,
		BaseType,
		Typedef,
		BaseClass,
		Friend,
		FunctionArgType,
		FuncDebugStart,
		FuncDebugEnd,
		UsingNamespace,
		VTableShape,
		VTable,
		Custom,
		Thunk,
		CustomType,
		ManagedType,
		Dimension,
		CallSite,
		InlineSite,
		BaseInterface,
		VectorType,
		MatrixType,
		HLSLType,
		Caller,
		Callee,
		Export,
		HeapAllocationSite,
		CoffGroup,
		Max
	}
}
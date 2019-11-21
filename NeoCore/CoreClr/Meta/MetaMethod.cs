using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using NeoCore.CoreClr.Meta.Base;
using NeoCore.CoreClr.VM;
using NeoCore.CoreClr.VM.Jit;
using NeoCore.Interop;
using NeoCore.Memory;
using NeoCore.Memory.Pointers;
using NeoCore.Utilities;
using NeoCore.Utilities.Extensions;
// ReSharper disable ReturnTypeCanBeEnumerable.Global

// ReSharper disable SuggestBaseTypeForParameter

// ReSharper disable InconsistentNaming

namespace NeoCore.CoreClr.Meta
{
	/// <summary>
	///     <list type="bullet">
	///         <item><description>CLR structure: <see cref="MethodDesc"/>, <see cref="MethodDescChunk"/></description></item>
	///         <item><description>Reflection structure: <see cref="MethodInfo"/></description></item>
	///     </list>
	/// </summary>
	public unsafe class MetaMethod : EmbeddedClrStructure<MethodDesc>
	{
		#region Constructors

		public MetaMethod(Pointer<MethodDesc> ptr) : base(ptr) { }

		public MetaMethod(MethodInfo member) : base(member) { }

		#endregion

		#region Accessors

		public int ChunkIndex => Value.Reference.ChunkIndex;

		public int SlotNumber => Value.Reference.SlotNumber;

		protected override Type[] AdditionalSources => new[] {typeof(MethodDescChunk)};

		#region bool

		public bool IsRuntimeSupplied =>
			Classification == MethodClassification.FCall ||
			Classification == MethodClassification.Array;

		public bool IsNoMetadata => Classification == MethodClassification.Dynamic;

		public bool HasILHeader => IsIL && !IsUnboxingStub && RVA > default(long);

		private bool IsUnboxingStub => Code.HasFlagFast(CodeFlags.IsUnboxingStub);

		public bool IsIL => MethodClassification.IL == Classification ||
		                    MethodClassification.Instantiated == Classification;

		public bool IsInlined {
			get {
				
				// https://github.com/dotnet/coreclr/blob/master/src/jit/inline.def
				// https://github.com/dotnet/coreclr/blob/master/src/jit/inline.cpp
				// https://github.com/dotnet/coreclr/blob/master/src/jit/inline.h
				// https://github.com/dotnet/coreclr/blob/master/src/jit/inlinepolicy.cpp
				// https://github.com/dotnet/coreclr/blob/master/src/jit/inlinepolicy.h
				// https://mattwarren.org/2016/03/09/adventures-in-benchmarking-method-inlining/
				
				throw new NotImplementedException();
			}
		}

		#endregion

		#region Flags

		public MethodClassification Classification => Value.Reference.Classification;
		public MethodProperties     Properties     => Value.Reference.Properties;
		public CodeFlags            Code           => Value.Reference.Code;
		public ParamFlags           ParameterTypes => Value.Reference.Flags3AndTokenRemainder;
		public MethodAttributes     Attributes     => MethodInfo.Attributes;

		#endregion

		public MethodInfo MethodInfo => (MethodInfo) Info;

		public override MemberInfo Info => (EnclosingType.RuntimeType).Module.ResolveMethod(Token);

		public void Prepare() => RuntimeHelpers.PrepareMethod(MethodInfo.MethodHandle);

		#endregion

		#region Delegates

		public long RVA => Value.Reference.RVA;

		public override MetaType EnclosingType => Value.Reference.MethodTable;

		public override int Token => Value.Reference.Token;

		public Pointer<byte> NativeCode {
			get => Value.Reference.NativeCode;
			set => Value.Reference.SetNativeCodeInterlocked(value.ToInt64());
		}

		public Pointer<byte> Function {
			get => MethodInfo.MethodHandle.GetFunctionPointer();
			set => NativeCode = value;
		}

		public void Reset() => Value.Reference.Reset();

		public bool IsPointingToNativeCode => Value.Reference.IsPointingToNativeCode();
		
		public MetaIL ILHeader {
			get {
				
				// bool
				const int ALLOW_OVERRIDES_PARAM = 0;
				return new MetaIL(Value.Reference.GetILHeader(ALLOW_OVERRIDES_PARAM));
			}
		}

		public Instruction[] Instructions {
			get {
				byte[] il = MethodInfo.GetMethodBody()?.GetILAsByteArray();
				return Functions.Inspection.ReadInstructions(il);
			}
		}

		#endregion

		#region Operators

		public static implicit operator MetaMethod(Pointer<MethodDesc> ptr) => new MetaMethod(ptr);

		public static implicit operator MetaMethod(MethodInfo t) => new MetaMethod(t);

		#endregion
	}
}
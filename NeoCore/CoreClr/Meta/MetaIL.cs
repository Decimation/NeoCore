using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Memkit.Pointers;
using NeoCore.CoreClr.Meta.Base;
using NeoCore.CoreClr.VM.Jit;
using NeoCore.Utilities;

// ReSharper disable InconsistentNaming

namespace NeoCore.CoreClr.Meta
{
	/// <summary>
	///     <list type="bullet">
	///         <item><description>CLR structure: <see cref="CorMethod"/>, <see cref="CorMethodTiny"/>, <see cref="CorMethodFat"/></description></item>
	///         <item><description>Reflection structure: <see cref="MethodBody"/></description></item>
	///     </list>
	/// </summary>
	public sealed class MetaIL : BasicClrStructure<CorMethod>
	{
		public MetaIL(Pointer<CorMethod> ptr) : base(ptr) { }

//		private MetaIL(MemberInfo member) : base(member) { }

		protected override Type[] AdditionalSources => new[] {typeof(CorMethodTiny), typeof(CorMethodFat)};

		/// <summary>
		/// <remarks>
		/// <para>Equals <see cref="MethodBody.LocalSignatureMetadataToken"/></para>
		/// </remarks>
		/// </summary>
		public int LocalVarSigToken => Value.Reference.LocalVarSigToken;

		/// <summary>
		/// <remarks>
		/// <para>Equals <see cref="MethodBody.MaxStackSize"/></para>
		/// </remarks>
		/// </summary>
		public int MaxStackSize => Value.Reference.MaxStackSize;

		/// <summary>
		/// <remarks>
		/// <para>Equals <see cref="MethodBody.GetILAsByteArray"/></para>
		/// </remarks>
		/// </summary>
		public byte[] CodeIL => Value.Reference.CodeIL;

		public Instruction[] Instructions => Instruction.ReadInstructions(CodeIL);
	}
}
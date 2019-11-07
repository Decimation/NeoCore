using System;
using System.Reflection;
using NeoCore.CoreClr.Components.VM.Jit;
using NeoCore.CoreClr.Meta.Base;
using NeoCore.Memory.Pointers;
// ReSharper disable InconsistentNaming

namespace NeoCore.CoreClr.Meta
{
	/// <summary>
	///     <list type="bullet">
	///         <item><description>CLR structure: <see cref="CorMethod"/>, <see cref="CorMethodTiny"/>, <see cref="CorMethodFat"/></description></item>
	///         <item><description>Reflection structure: <see cref="MethodBody"/></description></item>
	///     </list>
	/// </summary>
	public sealed class MetaIL : AnonymousClrStructure<CorMethod>
	{
		public MetaIL(Pointer<CorMethod> ptr) : base(ptr) { }

		private MetaIL(MemberInfo member) : base(member) { }

		protected override Type[] AdditionalSources => new[] {typeof(CorMethodTiny), typeof(CorMethodFat)};
	}
}
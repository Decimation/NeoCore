#region

using System;
using JetBrains.Annotations;

#endregion

namespace NeoCore.Interop.Attributes
{
	[AttributeUsage(AttributeTargets.Method)]
	[MeansImplicitUse(ImplicitUseTargetFlags.WithMembers)]
	public sealed class NativeFunctionAttribute : NativeAttribute { }
}
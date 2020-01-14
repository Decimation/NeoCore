#region

using System;
using JetBrains.Annotations;

#endregion

namespace NeoCore.Win32.Attributes
{
	[AttributeUsage(AttributeTargets.Method)]
	[MeansImplicitUse(ImplicitUseTargetFlags.WithMembers)]
	public sealed class NativeFunctionAttribute : Attribute {}
}
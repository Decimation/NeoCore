using System;
using JetBrains.Annotations;

namespace NeoCore.Interop.Attributes
{
	[AttributeUsage(AttributeTargets.Field)]
	[MeansImplicitUse(ImplicitUseTargetFlags.Itself)]
	public sealed class NativeFieldAttribute : NativeAttribute {}
}
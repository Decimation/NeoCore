using System;
using JetBrains.Annotations;

namespace NeoCore.Win32.Attributes
{
	[AttributeUsage(AttributeTargets.Field)]
	[MeansImplicitUse(ImplicitUseTargetFlags.Itself)]
	public sealed class NativeFieldAttribute : NativeAttribute {}
}
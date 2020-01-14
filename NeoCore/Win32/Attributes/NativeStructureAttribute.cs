using System;
using JetBrains.Annotations;

namespace NeoCore.Win32.Attributes
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
	[MeansImplicitUse(ImplicitUseTargetFlags.WithMembers)]
	public class NativeStructureAttribute : NativeAttribute {}
}
using System;
using JetBrains.Annotations;

namespace NeoCore.Interop.Attributes
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
	[MeansImplicitUse(ImplicitUseTargetFlags.WithMembers)]
	public sealed class NativeStructureAttribute : NativeAttribute
	{
		
	}
}
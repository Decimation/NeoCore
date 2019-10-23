using System;
using JetBrains.Annotations;

namespace NeoCore.Import.Attributes
{
	/// <summary>
	/// Shortcut to <see cref="ImportCallAttribute"/> with <see cref="IdentifierOptions.UseAccessorName"/> and
	/// <see cref="ImportCallOptions.Map"/>
	/// </summary>
	[MeansImplicitUse]
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Method)]
	public class ImportPropertyAttribute : ImportCallAttribute
	{
		public ImportPropertyAttribute() : base(IdentifierOptions.UseAccessorName, ImportCallOptions.Map) { }
	}
}
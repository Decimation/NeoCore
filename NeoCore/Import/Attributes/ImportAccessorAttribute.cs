using System;
using JetBrains.Annotations;

namespace NeoCore.Import.Attributes
{
	/// <summary>
	/// Shortcut to <see cref="ImportCallAttribute"/> with <see cref="IdentifierOptions.UseAccessorName"/> and
	/// <see cref="ImportCallOptions.Map"/> for use with <c>get</c> accessors.
	/// </summary>
	[MeansImplicitUse]
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Method)]
	public sealed class ImportAccessorAttribute : ImportCallAttribute
	{
		public ImportAccessorAttribute() : base(IdentifierOptions.UseAccessorName, ImportCallOptions.Map) { }
	}
}
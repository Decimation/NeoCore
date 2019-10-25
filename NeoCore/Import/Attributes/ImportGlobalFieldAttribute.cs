using System;
using JetBrains.Annotations;

namespace NeoCore.Import.Attributes
{
	/// <summary>
	/// Shortcut to <see cref="ImportFieldAttribute"/> with <see cref="IdentifierOptions.FullyQualified"/>
	/// for use with global variable imports.
	/// </summary>
	[MeansImplicitUse]
	[AttributeUsage(AttributeTargets.Field)]
	public sealed class ImportGlobalFieldAttribute : ImportFieldAttribute
	{
		public ImportGlobalFieldAttribute(ImportFieldOptions fieldOptions)
			: base(IdentifierOptions.FullyQualified, fieldOptions) { }
	}
}
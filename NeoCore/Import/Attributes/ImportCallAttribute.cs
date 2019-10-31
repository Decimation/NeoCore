using System;
using JetBrains.Annotations;
using NeoCore.Utilities;
using NeoCore.Utilities.Extensions;

namespace NeoCore.Import.Attributes
{
	[MeansImplicitUse]
	[AttributeUsage(METHOD_TARGETS)]
	public class ImportCallAttribute : ImportAttribute
	{
		internal const AttributeTargets METHOD_TARGETS = AttributeTargets.Method | AttributeTargets.Property;

		public ImportCallOptions CallOptions { get; set; } = ImportCallOptions.Map;

		public ImportCallAttribute() { }

		public ImportCallAttribute(ImportCallOptions callOptions)
		{
			CallOptions = callOptions;

			// Convenience
			if (callOptions.HasFlagFast(ImportCallOptions.Constructor)) {
				Options = IdentifierOptions.FullyQualified;
			}
		}

		public ImportCallAttribute(IdentifierOptions options, ImportCallOptions callOptions) : this(callOptions)
		{
			base.Options = options;
		}

		public ImportCallAttribute(IdentifierOptions options) : base(options) { }

		public ImportCallAttribute(string id, ImportCallOptions options) : this(options)
		{
			Identifier = id;
		}

		public ImportCallAttribute(string id, IdentifierOptions options = IdentifierOptions.None)
			: base(id, options) { }
	}
}
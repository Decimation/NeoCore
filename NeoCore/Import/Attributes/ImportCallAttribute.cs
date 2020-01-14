using System;
using JetBrains.Annotations;
using NeoCore.Utilities;
using NeoCore.Utilities.Extensions;
using NeoCore.Win32;

namespace NeoCore.Import.Attributes
{
	[MeansImplicitUse]
	[AttributeUsage(METHOD_TARGETS)]
	public class ImportCallAttribute : ImportAttribute
	{
		internal const AttributeTargets METHOD_TARGETS = AttributeTargets.Method | AttributeTargets.Property;

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

		public ImportCallOptions CallOptions { get; set; } = ImportCallOptions.Map;
	}
	
	/// <summary>
	/// Designates how the annotated function is imported.
	/// </summary>
	[Flags]
	public enum ImportCallOptions
	{
		None = 0,

		/// <summary>
		///     Treat the method as a constructor. <see cref="IdentifierOptions.FullyQualified" /> must be used.
		/// </summary>
		Constructor = 1,

		/// <summary>
		///     <para>
		///         Adds the resolved address to an <see cref="ImportMap"/> in the enclosing type
		///     </para>
		///     <para>
		///         The key of the import map is the name of the annotated member. The name is obtained using the
		///         <c>nameof</c> operator. The value is the resolved address.
		///     </para>
		///     <para>Best used in conjunction with the functions in <see cref="Functions.Native" /></para>
		/// </summary>
		Map = 1 << 1
	}
}
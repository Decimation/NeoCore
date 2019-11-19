using System;
using JetBrains.Annotations;

namespace NeoCore.Import.Attributes
{
	[MeansImplicitUse]
	[AttributeUsage(ImportFieldAttribute.FIELD_TARGETS | ImportCallAttribute.METHOD_TARGETS)]
	public abstract class ImportAttribute : Attribute
	{
		public string Identifier { get; protected set; }

		public IdentifierOptions Options { get; protected set; }

		public ImportAttribute() : this(IdentifierOptions.None) { }

		public ImportAttribute(IdentifierOptions options) : this(null, options) { }

		public ImportAttribute(string id, IdentifierOptions options = IdentifierOptions.None)
		{
			Identifier = id;
			Options    = options;
		}
	}
	
	/// <summary>
	/// Specifies how the identifier will be resolved.
	/// </summary>
	[Flags]
	public enum IdentifierOptions
	{
		None = 0,

		/// <summary>
		/// Don't use <see cref="ImportNamespaceAttribute.Namespace"/> in the identifier name resolution.
		/// </summary>
		IgnoreNamespace = 1,

		/// <summary>
		/// Don't use the enclosing type's name in the identifier name resolution.
		/// </summary>
		IgnoreEnclosingNamespace = 1 << 1,

		/// <summary>
		/// If the method is a <c>get</c> accessor, replace the <c>get_</c> in the name with <c>Get</c>
		/// </summary>
		UseAccessorName = 1 << 2,

		/// <summary>
		/// Use only the identifier name.
		/// <remarks>
		/// This is a combination of <see cref="IgnoreNamespace"/>, <see cref="IgnoreEnclosingNamespace"/>.
		/// This can also be used for global variables.
		/// </remarks>
		/// 
		/// </summary>
		FullyQualified = IgnoreNamespace | IgnoreEnclosingNamespace,
	}
}
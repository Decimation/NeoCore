using System;
using JetBrains.Annotations;
using NeoCore.CoreClr;
using NeoCore.Memory;
using NeoCore.Utilities;

namespace NeoCore.Import.Attributes
{
	[MeansImplicitUse]
	[AttributeUsage(FIELD_TARGETS)]
	public class ImportFieldAttribute : ImportAttribute
	{
		internal const AttributeTargets FIELD_TARGETS = AttributeTargets.Field;

		public ImportFieldAttribute() { }

		public ImportFieldAttribute(IdentifierOptions options, ImportFieldOptions fieldOptions) : this(options)
		{
			FieldOptions = fieldOptions;
		}

		public ImportFieldAttribute(IdentifierOptions options) : base(options) { }

		public ImportFieldAttribute(string id, IdentifierOptions options = IdentifierOptions.None)
			: base(id, options) { }

		/// <summary>
		/// The <see cref="Type"/> to load this field as. If left unset, the field will be interpreted as
		/// the target field's type.
		/// <remarks>To use this, <see cref="FieldOptions"/> must be <see cref="ImportFieldOptions.Proxy"/>.</remarks>
		/// </summary>
		public Type LoadAs { get; set; }

		/// <summary>
		/// Specifies how the target field will be loaded.
		/// </summary>
		public ImportFieldOptions FieldOptions { get; private set; } = ImportFieldOptions.Proxy;
	}
	
	
	

	/// <summary>
	/// Specifies how the field will be loaded.
	/// </summary>
	public enum ImportFieldOptions
	{
		/// <summary>
		/// Loads the value as the type specified by <see cref="ImportFieldAttribute.LoadAs"/>
		/// (or the field type if the type isn't specified)
		/// </summary>
		Proxy,

		Fast
	}
}
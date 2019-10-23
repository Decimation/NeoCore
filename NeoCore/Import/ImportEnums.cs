using System;
using NeoCore.Import.Attributes;

namespace NeoCore.Import
{
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
		///     Sets the method entry point to the resolved address. This only works on 64 bit.
		/// </summary>
		Bind = 1 << 1,

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
		Map = 1 << 2
	}
	
	/// <summary>
	/// Specifies how the field will be loaded.
	/// </summary>
	public enum ImportFieldOptions
	{
		/// <summary>
		/// Copy the value directly into the field from a byte array. Use <see cref="ImportFieldAttribute.SizeConst"/> to
		/// specify the size of the value. If the value isn't set, the size of the target field will be used.
		///
		/// <remarks>
		/// If the target field is a reference type, the memory is copied into the data (fields) of the object, not the
		/// pointer itself.
		/// </remarks>
		/// </summary>
		CopyIn,
		
		/// <summary>
		/// Loads the value as the type specified by <see cref="ImportFieldAttribute.LoadAs"/>
		/// (or the field type if the type isn't specified)
		/// </summary>
		Proxy,
		
		Fast
	}
}
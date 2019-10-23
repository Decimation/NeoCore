using System;
using JetBrains.Annotations;
using NeoCore.Interop.Attributes;

namespace NeoCore.Import.Attributes
{
	
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
	public sealed class ImportNamespaceAttribute : NativeStructureAttribute
	{
		public ImportNamespaceAttribute(string nameSpace)
		{
			Namespace = nameSpace;
		}

		public ImportNamespaceAttribute() : this(null) { }

		/// <summary>
		///     All members with <see cref="ImportAttribute" /> in the
		///     annotated class or struct will be prefixed with <see cref="Namespace" /> if the attribute has not
		///     set <seealso cref="IdentifierOptions.IgnoreNamespace" />
		/// </summary>
		public string Namespace { get; set; }
	}
}
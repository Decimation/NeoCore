using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace NeoCore.Import.Attributes
{
	/// <summary>
	/// Designates the <see cref="Dictionary{TKey,TValue}"/> to use for members specified with
	/// <see cref="ImportCallOptions.Map"/>
	/// </summary>
	[MeansImplicitUse]
	[AttributeUsage(AttributeTargets.Field)]
	public sealed class ImportMapDesignationAttribute : Attribute
	{
		
	}
}
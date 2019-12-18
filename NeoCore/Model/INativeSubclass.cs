namespace NeoCore.Model
{
	/// <summary>
	/// Specifies implicit native inheritance.
	/// <seealso cref="INativeStructure"/>
	/// <seealso cref="IStructure"/>
	/// <seealso cref="IWrapper{TNative}"/>
	/// <seealso cref="IClrStructure"/>
	/// </summary>
	/// <typeparam name="TSuper">Base structure</typeparam>
	public interface INativeSubclass<TSuper> where TSuper : INativeStructure { }
}
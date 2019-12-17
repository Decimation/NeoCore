namespace NeoCore.Model
{
	/// <summary>
	/// Specifies implicit native inheritance.
	/// </summary>
	/// <typeparam name="TSuper">Base structure</typeparam>
	public interface INativeInheritance<TSuper> where TSuper : INativeStructure { }
}
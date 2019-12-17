namespace NeoCore.Assets.Representation
{
	/// <summary>
	/// Represents a structure from a native source.
	/// </summary>
	public interface INativeStructure
	{
		/// <summary>
		/// Native name of this structure.
		/// </summary>
		string NativeName => GetType().Name;
	}
}
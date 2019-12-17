namespace NeoCore.Model
{
	/// <summary>
	/// Designates a native CLR metadata source structure.
	/// <para>https://github.com/dotnet/runtime</para>
	/// </summary>
	public interface IClrStructure : INativeStructure
	{
		/// <summary>
		/// The type of this structure. This should NOT be implemented as an auto-property
		/// to avoid changing the structure size.
		/// </summary>
		ClrStructureType Type { get; }
	}

	/// <summary>
	/// Designates the type of CLR structure.
	/// </summary>
	public enum ClrStructureType
	{
		/// <summary>
		/// The structure contains metadata.
		/// </summary>
		Metadata,

		/// <summary>
		/// The structure deals with memory management (e.g. GC).
		/// </summary>
		Memory,

		/// <summary>
		/// The structure is a utility/helper tool.
		/// </summary>
		Utility
	}
}
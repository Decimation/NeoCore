using NeoCore.Utilities.Diagnostics;

namespace NeoCore.Model
{
	/// <summary>
	/// Represents a structure or class that wraps a native structure.
	/// </summary>
	/// <typeparam name="TNative">Structure which this object wraps</typeparam>
	public interface IWrapper<out TNative> where TNative : INativeStructure
	{
		TNative TryGetNativeValue()
		{
			throw new NativeException("Cannot access the value of this structure.");
		}
	}
}
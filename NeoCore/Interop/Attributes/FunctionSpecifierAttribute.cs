using System;

namespace NeoCore.Interop.Attributes
{
	/// <summary>
	/// Designates the function to import with <see cref="Functions.Reflection.FindFunction{TDelegate}()"/>
	/// </summary>
	[AttributeUsage(AttributeTargets.Delegate)]
	public sealed class FunctionSpecifierAttribute : Attribute
	{
		public Type DeclaringType { get; set; }

		/// <summary>
		/// If <c>null</c>, the <see cref="Delegate"/> name will be used with
		/// substrings in <see cref="Functions.Reflection.DelegateNameRemoval"/> removed
		/// </summary>
		public string? Name { get; set; }

		public FunctionSpecifierAttribute(Type t, string? name = null)
		{
			DeclaringType = t;
			Name          = name;
		}

		public FunctionSpecifierAttribute() { }
	}
}
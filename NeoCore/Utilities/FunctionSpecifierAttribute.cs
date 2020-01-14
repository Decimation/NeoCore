using System;

namespace NeoCore.Utilities
{
	/// <summary>
	/// Designates the function to import with <see cref="Functions.Find{TDelegate}()"/>
	/// </summary>
	[AttributeUsage(AttributeTargets.Delegate)]
	public sealed class FunctionSpecifierAttribute : Attribute
	{
		public FunctionSpecifierAttribute(Type t, string? name = null)
		{
			DeclaringType = t;
			Name          = name;
		}

		public FunctionSpecifierAttribute() { }

		public Type DeclaringType { get; set; }

		/// <summary>
		/// If <c>null</c>, the <see cref="Delegate"/> name will be used with
		/// substrings in <see cref="Functions.DelegateNameRemoval"/> removed
		/// </summary>
		public string? Name { get; set; }
	}
}
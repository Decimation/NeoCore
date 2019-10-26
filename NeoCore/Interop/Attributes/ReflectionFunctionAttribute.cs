using System;

namespace NeoCore.Interop.Attributes
{
	[AttributeUsage(AttributeTargets.Delegate)]
	public sealed class ReflectionFunctionAttribute : Attribute
	{
		public Type DeclaringType { get; set; }
		
		public string Name { get; set; }
		
		public ReflectionFunctionAttribute(Type t, string name)
		{
			DeclaringType = t;
			Name = name;
		}

		public ReflectionFunctionAttribute()
		{
			
		}
	}
}
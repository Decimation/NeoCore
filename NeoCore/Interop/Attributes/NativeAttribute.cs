using System;

namespace NeoCore.Interop.Attributes
{
	public class NativeAttribute : Attribute
	{
		public string Id { get; }

		public NativeAttribute() : this(null) { }

		public NativeAttribute(string id)
		{
			Id = id;
		}
	}
}
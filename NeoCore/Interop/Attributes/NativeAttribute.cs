using System;

namespace NeoCore.Interop.Attributes
{
	public abstract class NativeAttribute : Attribute
	{
		public NativeAttribute() : this(null) { }

		public NativeAttribute(string id)
		{
			NativeId = id;
		}

		public string? NativeId { get; set; }

		public string[] Files { get; set; }
	}
}
using System;

namespace NeoCore.Interop.Attributes
{
	public abstract class NativeAttribute : Attribute
	{
		public string NativeId { get; set; }

		public string[] Files { get; set; }

		public NativeAttribute() : this(null) { }

		public NativeAttribute(string id)
		{
			NativeId = id;
		}
	}
}
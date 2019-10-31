using System;

namespace NeoCore.Utilities.Diagnostics
{
	internal sealed class NativeException : Exception
	{
		private const string ERROR = "Native error";

		public NativeException() : base(Guard.CreateErrorMessage(ERROR)) { }

		public NativeException(string msg) : base(Guard.CreateErrorMessage(msg)) { }
	}
}
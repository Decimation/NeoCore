using System;

namespace NeoCore.Utilities.Diagnostics
{
	internal sealed class NativeException : Exception
	{
		private const string ERROR = "Native error";
		
		internal NativeException() : base(Guard.CreateErrorMessage(ERROR)) { }
		
		internal NativeException(string msg) : base(Guard.CreateErrorMessage(msg)) { }
	}
}
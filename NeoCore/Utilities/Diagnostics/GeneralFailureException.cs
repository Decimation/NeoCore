using System;

namespace NeoCore.Utilities.Diagnostics
{
	internal sealed class GeneralFailureException : Exception
	{
		// todo: this name should be more specific
		
		private const string ERROR = "Failure";
		
		internal GeneralFailureException() : base(Guard.CreateErrorMessage(ERROR)) { }
		
		internal GeneralFailureException(string msg) : base(Guard.CreateErrorMessage(msg)) { }
	}
}
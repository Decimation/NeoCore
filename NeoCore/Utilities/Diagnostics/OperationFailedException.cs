using System;

namespace NeoCore.Utilities.Diagnostics
{
	internal sealed class OperationFailedException : Exception
	{
		// todo: this name should be more specific
		
		private const string ERROR = "Failure";
		
		internal OperationFailedException() : base(Guard.CreateErrorMessage(ERROR)) { }
		
		internal OperationFailedException(string msg) : base(Guard.CreateErrorMessage(msg)) { }
	}
}
using System;

namespace NeoCore.Utilities.Diagnostics
{
	internal sealed class GuardException : Exception
	{
		private const string ERROR = "Assertion failed";
		
		internal GuardException() : base(Guard.CreateErrorMessage(ERROR)) { }
		
		internal GuardException(string msg) : base(Guard.CreateErrorMessage(msg)) { }
	}
}
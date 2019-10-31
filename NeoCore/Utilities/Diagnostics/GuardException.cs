using System;

namespace NeoCore.Utilities.Diagnostics
{
	internal sealed class GuardException : Exception
	{
		private const string ERROR = "Assertion failed";

		public GuardException() : base(Guard.CreateErrorMessage(ERROR)) { }

		public GuardException(string msg) : base(Guard.CreateErrorMessage(msg)) { }
	}
}
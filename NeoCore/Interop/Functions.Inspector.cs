using System;

namespace NeoCore.Interop
{
	public static partial class Functions
	{
		public static class Inspector
		{
			public static bool FunctionThrows<TException>(Action action) where TException : Exception
			{
				try {
					action();
					return false;
				}
				catch (TException) {
					return true;
				}
			}
		}
	}
}
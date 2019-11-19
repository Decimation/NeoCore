#region

using System;

#endregion

namespace NeoCore.Interop
{
	public static partial class Functions
	{
		/// <summary>
		/// Contains utilities for inspecting functions.
		/// </summary>
		public static class Inspection
		{
			/// <summary>
			///     Determines whether <paramref name="action" /> throws <typeparamref name="TException" /> when
			///     executed.
			/// </summary>
			/// <param name="action">Action to run</param>
			/// <typeparam name="TException">Type of <see cref="Exception" /> to check for</typeparam>
			/// <returns>
			///     <c>true</c> if <paramref name="action" /> throws <typeparamref name="TException" /> when executed;
			///     <c>false</c> otherwise
			/// </returns>
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
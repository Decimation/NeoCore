using System;
using System.Diagnostics;
using JetBrains.Annotations;
using NeoCore.Assets;
using NeoCore.CoreClr;
using NeoCore.Memory;

namespace NeoCore.Utilities.Diagnostics
{
	internal static class Guard
	{
		private const string VALUE_NULL_HALT = "value:null => halt";

		private const string COND_FALSE_HALT = "condition:false => halt";

		private const string UNCONDITIONAL_HALT = "=> halt";


		#region Assert

		[AssertionMethod]
		[ContractAnnotation(COND_FALSE_HALT)]
		[Conditional(Constants.COND_DEBUG)]
		internal static void AssertDebug(bool condition)
		{
			if (!condition) {
				throw new GuardException();
			}
		}

		[AssertionMethod]
		[ContractAnnotation(COND_FALSE_HALT)]
		internal static void Assert(bool condition)
		{
			if (!condition) {
				throw new GuardException();
			}
		}

		[AssertionMethod]
		[ContractAnnotation(COND_FALSE_HALT)]
		[StringFormatMethod(Constants.STRING_FMT_ARG)]
		internal static void Assert(bool condition, string msg = null, params object[] args)
		{
			if (!condition) {
				if (msg != null) {
					throw new GuardException(String.Format(msg, args));
				}

				throw new GuardException();
			}
		}

		/// <summary>
		///     Checks compatibility
		/// </summary>
		internal static void AssertCompatibility()
		{
			Guard.Assert(Runtime.Info.IsWindowsPlatform);
			Guard.Assert(Runtime.Info.IsWorkstationGC);
			Guard.Assert(!Runtime.Info.IsMonoRuntime);
		}

		#endregion

		#region Null check

		[AssertionMethod]
		[ContractAnnotation(VALUE_NULL_HALT)]
		internal static void AssertNotNull<T>(T value, string name = null) where T : class
		{
			if (value == null) {
				if (name == null) {
					throw new ArgumentNullException();
				}

				throw new ArgumentNullException(name);
			}
		}

		[AssertionMethod]
		[ContractAnnotation(VALUE_NULL_HALT)]
		internal static void AssertNotNull(Pointer<byte> value, string name = null)
		{
			if (value.IsNull) {
				if (name == null) {
					throw new ArgumentNullException();
				}

				throw new ArgumentNullException(name);
			}
		}

		#endregion

		[AssertionMethod]
		[ContractAnnotation(UNCONDITIONAL_HALT)]
		internal static void Fail(string msg = null)
		{
			if (msg == null) {
				throw new OperationFailedException();
			}

			throw new OperationFailedException(msg);
		}

		internal static string CreateErrorMessage(string template, string msg = null)
		{
			return msg == null ? template : String.Format("{0}: {1}", template, msg);
		}
	}
}
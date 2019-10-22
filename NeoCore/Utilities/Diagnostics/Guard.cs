using System;
using System.Diagnostics;
using JetBrains.Annotations;
using NeoCore.Memory;

namespace NeoCore.Utilities.Diagnostics
{
	internal static class Guard
	{
		private const string VALUE_NULL_HALT = "value:null => halt";

		private const string COND_FALSE_HALT = "condition:false => halt";

		private const string UNCONDITIONAL_HALT = "=> halt";

		private const string STRING_FMT_ARG = "msg";

		private const string CONDITIONAL_DEBUG = "DEBUG";

		[AssertionMethod]
		[ContractAnnotation(COND_FALSE_HALT)]
		[Conditional(CONDITIONAL_DEBUG)]
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
		[StringFormatMethod(STRING_FMT_ARG)]
		internal static void Assert(bool condition, string msg = null, params object[] args)
		{
			if (!condition) {
				if (msg != null) {
					throw new GuardException(String.Format(msg, args));
				}

				throw new GuardException();
			}
		}

		[AssertionMethod]
		[ContractAnnotation(VALUE_NULL_HALT)]
		internal static void NullCheck<T>(T value, string name) where T : class
		{
			if (value == null) {
				throw new ArgumentNullException(name);
			}
		}

		[AssertionMethod]
		[ContractAnnotation(VALUE_NULL_HALT)]
		internal static void NullCheck(Pointer<byte> value, string name)
		{
			if (value.IsNull) {
				throw new ArgumentNullException(name);
			}
		}

		[AssertionMethod]
		[ContractAnnotation(UNCONDITIONAL_HALT)]
		internal static void Fail(string msg = null)
		{
			if (msg == null) {
				throw new GeneralFailureException();
			}

			throw new GeneralFailureException(msg);
		}

		internal static string CreateErrorMessage(string template, string msg = null)
		{
			return msg == null ? template : String.Format("{0}: {1}", template, msg);
		}
	}
}
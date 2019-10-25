using System;
using System.Diagnostics;
using JetBrains.Annotations;
using NeoCore.Assets;
using NeoCore.CoreClr;
using NeoCore.Memory;
using NeoCore.Memory.Pointers;

namespace NeoCore.Utilities.Diagnostics
{
	internal static class Guard
	{
		private const string VALUE_NULL_HALT = "value:null => halt";
		
		private const string VALUE_NOTNULL_HALT = "value:notnull => halt";
		
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
			Assert(Runtime.Info.IsWindowsPlatform
			       && Runtime.Info.IsWorkstationGC
			       && !Runtime.Info.IsMonoRuntime);
		}

		#endregion

		#region Null check

		[AssertionMethod]
		[ContractAnnotation(VALUE_NOTNULL_HALT)]
		internal static void AssertNull<T>(T value, string name = null) where T : class
		{
			if (value != null) {
				if (name == null) {
					throw new GuardException();
				}

				throw new GuardException(name);
			}
		}
		
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
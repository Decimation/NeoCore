using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
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

		internal const string COND_DEBUG = "DEBUG";

		private const string STRING_FMT_ARG = "msg";

		#region Assert

		[AssertionMethod]
		[ContractAnnotation(COND_FALSE_HALT)]
		[Conditional(COND_DEBUG)]
		internal static void AssertDebug(bool condition) => Assert<GuardException>(condition);

		[AssertionMethod]
		[ContractAnnotation(COND_FALSE_HALT)]
		internal static void Assert(bool condition) => Assert<GuardException>(condition);

		[AssertionMethod]
		[ContractAnnotation(COND_FALSE_HALT)]
		[StringFormatMethod(STRING_FMT_ARG)]
		internal static void Assert<TException>(bool condition, string? msg = null, params object[] args)
			where TException : Exception, new()
		{
			if (!condition) {
				Fail<TException>(msg,args);
			}
		}

		[AssertionMethod]
		[ContractAnnotation(COND_FALSE_HALT)]
		[StringFormatMethod(STRING_FMT_ARG)]
		internal static void Assert(bool condition, string? msg = null, params object[] args) =>
			Assert<GuardException>(condition, msg, args);

		[AssertionMethod]
		[ContractAnnotation(COND_FALSE_HALT)]
		[StringFormatMethod(STRING_FMT_ARG)]
		internal static void AssertWin32(bool condition, string? msg = null, params object[] args) =>
			Assert<Win32Exception>(condition, msg, args);

		/// <summary>
		///     Checks compatibility
		/// </summary>
		internal static void AssertCompatibility()
		{
			Assert(Runtime.IsWindowsPlatform
			       && Runtime.IsWorkstationGC && Runtime.CurrentFramework == ClrFrameworks.Core);
		}

		#endregion

		#region Null check

		[AssertionMethod]
		[ContractAnnotation(VALUE_NOTNULL_HALT)]
		internal static void AssertNull<T>(T value, string? name = null) where T : class =>
			Assert<ArgumentNullException>(value == null, name);

		[AssertionMethod]
		[ContractAnnotation(VALUE_NULL_HALT)]
		internal static void AssertNotNull<T>(T value, string? name = null) where T : class =>
			Assert<ArgumentNullException>(value != null, name);

		[AssertionMethod]
		[ContractAnnotation(VALUE_NULL_HALT)]
		internal static void AssertNotNull(Pointer<byte> value, string? name = null) =>
			Assert<ArgumentNullException>(!value.IsNull, name);

		#endregion

		[AssertionMethod]
		[ContractAnnotation(UNCONDITIONAL_HALT)]
		[StringFormatMethod(STRING_FMT_ARG)]
		internal static void Fail<TException>(string? msg = null, params object[] args)
			where TException : Exception, new()
		{
			if (msg != null) {
				msg = String.Format(msg, args);

				throw (TException) Activator.CreateInstance(typeof(TException), msg);
			}

			throw new TException();
		}

		[AssertionMethod]
		[ContractAnnotation(UNCONDITIONAL_HALT)]
		[StringFormatMethod(STRING_FMT_ARG)]
		internal static void Fail(string? msg = null, params object[] args) => Fail<GuardException>(msg, args);

		internal static string CreateErrorMessage(string template, string? msg = null)
		{
			return msg == null ? template : String.Format("{0}: {1}", template, msg);
		}
	}
}
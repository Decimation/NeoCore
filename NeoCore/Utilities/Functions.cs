using System;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Memkit.Pointers;
using NeoCore.Utilities.Diagnostics;

namespace NeoCore.Utilities
{
	/// <summary>
	/// Contains various utilities for working with native (unmanaged) and managed functions.
	/// 
	/// Also contains delegates for managed internal System functions which interact between the managed-unmanaged
	/// (System-CLR) boundary, and shortcuts for Reflection and utilities
	/// for working with managed functions.
	/// 
	/// </summary>
	public static partial class Functions
	{
		/// <summary>
		/// Strings to remove from <see cref="Delegate"/> member names annotated with <see cref="FunctionImportAttribute"/>
		/// </summary>
		private static readonly string[] DelegateNameRemoval = {"Delegate"};

		public static bool Throws(Action action)
		{
			try {
				action();
				return false;
			}
			catch {
				return true;
			}
		}

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
		public static bool Throws<TException>(Action action) where TException : Exception
		{
			try {
				action();
				return false;
			}
			catch (TException) {
				return true;
			}
		}

		public static TDelegate Find<TDelegate>(Pointer<byte> h, string s) where TDelegate : Delegate
		{
			var f  = Memkit.Interop.Native.GetProcAddress(h.Address, s).Address;
			var fn = Marshal.GetDelegateForFunctionPointer<TDelegate>(f);
			return fn;
		}

		public static Delegate Find(Pointer<byte> h, string s, Type t)
		{
			var f  = Memkit.Interop.Native.GetProcAddress(h.Address, s).Address;
			var fn = Marshal.GetDelegateForFunctionPointer(f, t);
			return fn;
		}

		public static TDelegate Find<TDelegate>() where TDelegate : Delegate
		{
			var attr = typeof(TDelegate).GetCustomAttribute<FunctionImportAttribute>();
			Guard.AssertNotNull(attr);

			string? nameStub = attr.Name ?? typeof(TDelegate).Name;
			string? name = DelegateNameRemoval.Aggregate(
				nameStub, (current, remove) =>
					current.Replace(remove, String.Empty));

			return Find<TDelegate>(attr.DeclaringType, name);
		}

		public static TDelegate Find<TDelegate, TSource>(string name) where TDelegate : Delegate =>
			Find<TDelegate>(typeof(TSource), name);

		public static TDelegate Find<TDelegate>(Type type, string name) where TDelegate : Delegate
		{
			var method = type.GetAnyMethod(name);

			return (TDelegate) method.CreateDelegate(typeof(TDelegate));
		}

		#region Generic

		/// <summary>
		///     Executes a generic method
		/// </summary>
		/// <param name="method">Method to execute</param>
		/// <param name="args">Generic type parameters</param>
		/// <param name="value">Instance of type; <c>null</c> if the method is static</param>
		/// <param name="fnArgs">Method arguments</param>
		/// <returns>Return value of the method specified by <paramref name="method"/></returns>
		public static object CallGeneric(MethodInfo method, Type[]          args,
		                                 object     value,  params object[] fnArgs)
		{
			return method.MakeGenericMethod(args).Invoke(value, fnArgs);
		}

		public static object CallGeneric(MethodInfo method, Type            arg,
		                                 object     value,  params object[] fnArgs)
		{
			return method.MakeGenericMethod(arg).Invoke(value, fnArgs);
		}

		#endregion
	}
}
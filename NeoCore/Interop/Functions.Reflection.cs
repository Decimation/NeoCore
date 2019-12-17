using System;
using System.Reflection;
using NeoCore.Interop.Attributes;
using NeoCore.Utilities;
using NeoCore.Utilities.Diagnostics;
using NeoCore.Utilities.Extensions;

namespace NeoCore.Interop
{
	public static unsafe partial class Functions
	{
		/// <summary>
		/// Contains delegates for managed internal System functions which interact between the managed-unmanaged
		/// (System-CLR) boundary. Also contains shortcuts for Reflection and utilities
		/// for working with managed functions.
		/// </summary>
		public static class Reflection
		{
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

			public static TDelegate FindFunction<TDelegate>() where TDelegate : Delegate
			{
				var attr = typeof(TDelegate).GetCustomAttribute<ReflectionFunctionAttribute>();
				Guard.AssertNotNull(attr);
				return FindFunction<TDelegate>(attr.DeclaringType, attr.Name);
			}

			public static TDelegate FindFunction<TDelegate, TSource>(string name) where TDelegate : Delegate =>
				FindFunction<TDelegate>(typeof(TSource), name);

			public static TDelegate FindFunction<TDelegate>(Type type, string name) where TDelegate : Delegate
			{
				var method = type.GetAnyMethod(name);

				return (TDelegate) method.CreateDelegate(typeof(TDelegate));
			}
		}
	}
}
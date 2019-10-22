#region

using System;
using System.Diagnostics;
using InlineIL;
using JetBrains.Annotations;
using NeoCore.CoreClr.Framework;
using NeoCore.Interop.Attributes;
using NeoCore.Memory;
using NeoCore.Utilities;

#endregion

namespace NeoCore.CoreClr
{
	public static partial class Runtime
	{
		/// <summary>
		/// Contains utilities for retrieving information about the .NET runtime and its components.
		/// </summary>
		public static class Info
		{
			public static bool IsInDebugMode => Debugger.IsAttached;

			public static FrameworkIdentifier CurrentFramework {
				get {
					FrameworkTypes fwk = default;
					Version        vsn = null;

#if NETCOREAPP
					fwk = FrameworkTypes.Core;

#if NETCOREAPP3_0
					vsn = new Version(3, 0);
#endif

#endif

#if NETFRAMEWORK
					fwk = NetFrameworks.Framework;
#endif

#if NETSTANDARD
					fwk = NetFrameworks.Standard;
#endif

					return new FrameworkIdentifier(fwk, vsn);
				}
			}

			public static bool IsBoxed<T>(T value)
			{
				return (typeof(T).IsInterface || typeof(T) == typeof(object)) && value != null && IsStruct(value);
			}

			public static bool IsPinnable<T>(T value) where T : class
			{
				// https://github.com/dotnet/coreclr/blob/master/src/vm/marshalnative.cpp#L257

				throw new NotImplementedException();
			}


			/// <summary>
			/// Whether the value of <paramref name="value"/> is <c>default</c> or <c>null</c> bytes,
			/// or <paramref name="value"/> is <c>null</c>
			///
			/// <remarks>"Nil" is <c>null</c> or <c>default</c>.</remarks>
			/// </summary>
			[NativeFunction]
			public static bool IsNil<T>([CanBeNull] T value)
			{
				/*public static bool IsNullOrDefault<T>([CanBeNull] T value)
				{
					return EqualityComparer<T>.Default.Equals(value, default);
				}*/

				// Fastest method for calculating whether a value is nil.
				IL.Emit.Ldarg(nameof(value));
				IL.Emit.Ldnull();
				IL.Emit.Ceq();
				IL.Emit.Ret();
				return IL.Return<bool>();
			}
			
			internal static bool IsString<T>(T value) => value is string;
			
			internal static bool IsArray<T>(T value) => value is Array;
			
			internal static bool IsStruct<T>(T value) => value.GetType().IsValueType;
		}
	}
}
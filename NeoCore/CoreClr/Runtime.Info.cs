#region

using System;
using System.Collections;
using System.Diagnostics;
using System.Runtime;
using System.Runtime.InteropServices;
using InlineIL;
using JetBrains.Annotations;
using NeoCore.CoreClr.Support;
using NeoCore.Interop.Attributes;
using NeoCore.Memory;
using NeoCore.Utilities;
using NeoCore.Utilities.Diagnostics;

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedTypeParameter

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

			public static bool IsWindowsPlatform => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

			public static bool IsWorkstationGC => !GCSettings.IsServerGC;

			public static bool IsMonoRuntime => Type.GetType("Mono.Runtime") != null;

			public static ClrFramework CurrentFramework {
				get {
#if NETCOREAPP
					return ClrFrameworks.Core;

#endif

#if NETFRAMEWORK
					return ClrFrameworks.Framework;
#endif

#if NETSTANDARD
					return ClrFrameworks.Standard;
#endif

					Guard.Fail();
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

			public static bool IsInteger(Type t)
			{
				return Type.GetTypeCode(t) switch
				{
					TypeCode.Byte => true,
					TypeCode.SByte => true,
					TypeCode.UInt16 => true,
					TypeCode.Int16 => true,
					TypeCode.UInt32 => true,
					TypeCode.Int32 => true,
					TypeCode.UInt64 => true,
					TypeCode.Int64 => true,
					_ => false
				};
			}

			public static bool IsReal(Type t)
			{
				return Type.GetTypeCode(t) switch
				{
					TypeCode.Decimal => true,
					TypeCode.Double => true,
					TypeCode.Single => true,
					_ => false
				};
			}

			#region Unmanaged

			/// <summary>
			/// Dummy class for use with <see cref="IsUnmanaged"/> and <see cref="IsUnmanaged"/>
			/// </summary>
			private sealed class U<T> where T : unmanaged { }

			/// <summary>
			/// Determines whether this type fits the <c>unmanaged</c> type constraint.
			/// </summary>
			public static bool IsUnmanaged(Type t)
			{
				try {
					// ReSharper disable once ReturnValueOfPureMethodIsNotUsed
					typeof(U<>).MakeGenericType(t);
					return true;
				}
				catch {
					return false;
				}
			}

			public static bool ImplementsInterface(Type type, string name) => type.GetInterface(name) != null;

			public static bool IsEnumerableType(Type type) => ImplementsInterface(type, nameof(IEnumerable));

			#endregion


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
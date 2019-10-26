#region

using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Runtime;
using System.Runtime.InteropServices;
using InlineIL;
using JetBrains.Annotations;
using NeoCore.CoreClr.Meta;
using NeoCore.CoreClr.Support;
using NeoCore.Interop.Attributes;
using NeoCore.Memory.Pointers;
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
#pragma warning disable 162
					// ReSharper disable once HeuristicUnreachableCode
					Guard.Fail();
#pragma warning restore 162
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

			internal static AuxiliaryProperties ReadProperties(Type t)
			{
				var mp = new AuxiliaryProperties();

				if (IsInteger(t)) {
					mp |= AuxiliaryProperties.Integer;
				}

				if (IsReal(t)) {
					mp |= AuxiliaryProperties.Real;
				}

				if (IsStruct(t)) {
					mp |= AuxiliaryProperties.Struct;
				}

				if (IsUnmanaged(t)) {
					mp |= AuxiliaryProperties.Unmanaged;
				}

				if (IsEnumerableType(t)) {
					mp |= AuxiliaryProperties.Enumerable;
				}

				if (IsAnyPointer(t)) {
					mp |= AuxiliaryProperties.AnyPointer;
				}

				if (t.IsPointer) {
					mp |= AuxiliaryProperties.Pointer;
				}

				return mp;
			}

			private static bool IsInteger(Type t)
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

			private static bool IsReal(Type t)
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
			private static bool IsUnmanaged(Type t)
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

			#endregion

			private static bool IsAnyPointer(Type t)
			{
				bool isIPointer = ImplementsGenericInterface(t, typeof(IPointer<>));
				bool isIntPtr   = t == typeof(IntPtr) || t == typeof(UIntPtr);

				return t.IsPointer || isIPointer || isIntPtr;
			}

			private static bool IsEnumerableType(Type type) => ImplementsInterface(type, nameof(IEnumerable));

			public static bool ImplementsGenericInterface(Type type, Type interfaceType)
			{
				return type.GetInterfaces().Any(x => x.IsGenericType
				                                     && x.GetGenericTypeDefinition() == interfaceType);
			}

			public static bool ImplementsInterface(Type type, string name) => type.GetInterface(name) != null;

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
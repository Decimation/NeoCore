#region

using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Runtime;
using System.Runtime.InteropServices;
using InlineIL;
using JetBrains.Annotations;
using NeoCore.CoreClr.Components.Support;
using NeoCore.CoreClr.Components.VM;
using NeoCore.CoreClr.Meta;
using NeoCore.Interop;
using NeoCore.Interop.Attributes;
using NeoCore.Memory.Pointers;
using NeoCore.Utilities.Diagnostics;
using NeoCore.Utilities.Extensions;

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

			[Obsolete]
			public static bool IsPinnableSlow(object value)
			{
				return !FunctionThrows<ArgumentException>(() =>
				{
					var gc = GCHandle.Alloc(value, GCHandleType.Pinned);
					gc.Free();
				});
			}

			public static bool IsPinnable([CanBeNull] object value)
			{
				// https://github.com/dotnet/coreclr/blob/master/src/vm/marshalnative.cpp#L280

				/*var throws = !CheckFunctionThrow<ArgumentException>(() =>
				{
					var gc = GCHandle.Alloc(value, GCHandleType.Pinned);
					gc.Free();
				});*/

				// return Functions.Clr.IsPinnable(value);

				if (value == null) {
					return true;
				}

				var mt = ReadTypeHandle(value);

				if (IsString(value)) {
					return true;
				}

				if (mt.IsArray) {
					var  corType         = mt.ElementTypeHandle.NormType;
					bool isPrimitiveElem = ClrSigs.IsPrimitiveType(corType);

					if (isPrimitiveElem) {
						return true;
					}

					var th = mt.ElementTypeHandle;

					if (!th.IsTypeDesc) {
						if (th.IsStruct && th.IsBlittable) {
							return true;
						}
					}

					return false;
				}

				return mt.IsBlittable;
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

				if (t.IsValueType) {
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
				return !FunctionThrows<Exception>(() =>
				{
					// ReSharper disable once ReturnValueOfPureMethodIsNotUsed
					typeof(U<>).MakeGenericType(t);
				});
			}

			#endregion

			private static bool IsAnyPointer(Type t)
			{
				bool isIPointer = t.ImplementsGenericInterface(typeof(IPointer<>));
				bool isIntPtr   = t == typeof(IntPtr) || t == typeof(UIntPtr);

				return t.IsPointer || isIPointer || isIntPtr;
			}

			private static bool IsEnumerableType(Type type) => type.ImplementsInterface(nameof(IEnumerable));

			/// <summary>
			/// Determines whether the value of <paramref name="value"/> is <c>default</c> or <c>null</c> bytes,
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

			/// <summary>
			/// Determines whether or not <paramref name="value"/> is a runtime <see cref="string"/>.
			/// </summary>
			/// <param name="value">Value to test</param>
			/// <typeparam name="T">Type to test</typeparam>
			/// <returns><c>true</c> if <paramref name="value"/> is a <see cref="string"/>; <c>false</c> otherwise</returns>
			internal static bool IsString<T>(T value) => value is string;

			/// <summary>
			/// Determines whether or not <paramref name="value"/> is a runtime <see cref="Array"/>.
			/// </summary>
			/// <param name="value">Value to test</param>
			/// <typeparam name="T">Type to test</typeparam>
			/// <returns><c>true</c> if <paramref name="value"/> is an <see cref="Array"/>; <c>false</c> otherwise</returns>
			internal static bool IsArray<T>(T value) => value is Array;

			/// <summary>
			/// Determines whether or not <paramref name="value"/> is a runtime value type.
			/// </summary>
			/// <param name="value">Value to test</param>
			/// <typeparam name="T">Type to test</typeparam>
			/// <returns><c>true</c> if <paramref name="value"/> is a value type; <c>false</c> otherwise</returns>
			internal static bool IsStruct<T>(T value) => value.GetType().IsValueType;

			/// <summary>
			/// Determines whether or not <typeparamref name="T"/> is a compile-time <see cref="string"/>.
			/// </summary>
			/// <typeparam name="T">Type to test</typeparam>
			/// <returns><c>true</c> if <typeparamref name="T"/> is a <see cref="string"/>; <c>false</c> otherwise</returns>
			internal static bool IsCompileString<T>() => typeof(T) == typeof(string);

			/// <summary>
			/// Determines whether or not <typeparamref name="T"/> is a compile-time <see cref="Array"/>.
			/// </summary>
			/// <typeparam name="T">Type to test</typeparam>
			/// <returns><c>true</c> if <typeparamref name="T"/> is an <see cref="Array"/>; <c>false</c> otherwise</returns>
			internal static bool IsCompileArray<T>() => typeof(T).IsArray || typeof(T) == typeof(Array);

			/// <summary>
			/// Determines whether or not <typeparamref name="T"/> is a compile-time value type.
			/// </summary>
			/// <typeparam name="T">Type to test</typeparam>
			/// <returns><c>true</c> if <typeparamref name="T"/> is a value type; <c>false</c> otherwise</returns>
			internal static bool IsCompileStruct<T>() => typeof(T).IsValueType;
		}
	}
}
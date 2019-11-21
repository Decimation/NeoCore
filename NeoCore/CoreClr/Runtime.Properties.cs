using System;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using NeoCore.CoreClr.VM;
using NeoCore.Interop;
using NeoCore.Memory.Pointers;
using NeoCore.Utilities.Extensions;

namespace NeoCore.CoreClr
{
	public static partial class Runtime
	{
		/// <summary>
		/// Contains utilities for inspecting <see cref="Type"/> properties.
		/// </summary>
		internal static class Properties
		{
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

			#region Unmanaged

			/// <summary>
			///     Dummy class for use with <see cref="IsUnmanaged" /> and <see cref="IsUnmanaged" />
			/// </summary>
			private sealed class U<T> where T : unmanaged { }

			/// <summary>
			///     Determines whether this type fits the <c>unmanaged</c> type constraint.
			/// </summary>
			private static bool IsUnmanaged(Type t)
			{
				return !Functions.Inspection.FunctionThrows<Exception>(() =>
				{
					// ReSharper disable once ReturnValueOfPureMethodIsNotUsed
					typeof(U<>).MakeGenericType(t);
				});
			}

			#endregion

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

			private static bool IsAnyPointer(Type t)
			{
				bool isIPointer = t.ImplementsGenericInterface(typeof(IPointer<>));
				bool isIntPtr   = t == typeof(IntPtr) || t == typeof(UIntPtr);

				return t.IsPointer || isIPointer || isIntPtr;
			}

			private static bool IsEnumerableType(Type type) => type.ImplementsInterface(nameof(IEnumerable));

			/// <summary>
			///     Determines whether or not <typeparamref name="T" /> is a compile-time value type.
			/// </summary>
			/// <typeparam name="T">Type to test</typeparam>
			/// <returns><c>true</c> if <typeparamref name="T" /> is a value type; <c>false</c> otherwise</returns>
			internal static bool IsCompileStruct<T>() => typeof(T).IsValueType;

			/// <summary>
			///     Determines whether or not <typeparamref name="T" /> is a compile-time <see cref="Array" />.
			/// </summary>
			/// <typeparam name="T">Type to test</typeparam>
			/// <returns><c>true</c> if <typeparamref name="T" /> is an <see cref="Array" />; <c>false</c> otherwise</returns>
			internal static bool IsCompileArray<T>() => typeof(T).IsArray || typeof(T) == typeof(Array);

			/// <summary>
			///     Determines whether or not <typeparamref name="T" /> is a compile-time <see cref="string" />.
			/// </summary>
			/// <typeparam name="T">Type to test</typeparam>
			/// <returns><c>true</c> if <typeparamref name="T" /> is a <see cref="string" />; <c>false</c> otherwise</returns>
			internal static bool IsCompileString<T>() => typeof(T) == typeof(string);

			/// <summary>
			///     Determines whether or not <paramref name="value" /> is a runtime value type.
			/// </summary>
			/// <param name="value">Value to test</param>
			/// <typeparam name="T">Type to test</typeparam>
			/// <returns><c>true</c> if <paramref name="value" /> is a value type; <c>false</c> otherwise</returns>
			internal static bool IsStruct<T>(T value) => value.GetType().IsValueType;


			/// <summary>
			///     Determines whether or not <paramref name="value" /> is a runtime <see cref="Array" />.
			/// </summary>
			/// <param name="value">Value to test</param>
			/// <typeparam name="T">Type to test</typeparam>
			/// <returns><c>true</c> if <paramref name="value" /> is an <see cref="Array" />; <c>false</c> otherwise</returns>
			internal static bool IsArray<T>(T value) => value is Array;

			/// <summary>
			///     Determines whether or not <paramref name="value" /> is a runtime <see cref="string" />.
			/// </summary>
			/// <param name="value">Value to test</param>
			/// <typeparam name="T">Type to test</typeparam>
			/// <returns><c>true</c> if <paramref name="value" /> is a <see cref="string" />; <c>false</c> otherwise</returns>
			internal static bool IsString<T>(T value) => value is string;
		}
	}
}
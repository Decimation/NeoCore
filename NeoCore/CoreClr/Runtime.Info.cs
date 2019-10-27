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

			private const ElementType PRIMITIVE_TABLE_SIZE = ElementType.String;

			private const int PT_Primitive = 0x01000000;

			/// <summary>
			/// <para>The Attributes Table</para>
			/// <para>20 bits for built in types and 12 bits for Properties</para>
			/// <para>The properties are followed by the widening mask. All types widen to themselves.</para>
			/// <para>https://github.com/dotnet/coreclr/blob/master/src/vm/invokeutil.cpp</para>
			/// <para>https://github.com/dotnet/coreclr/blob/master/src/vm/invokeutil.h</para>
			/// </summary>
			private static readonly int[] PrimitiveAttributes =
			{
				0x00,                  // ELEMENT_TYPE_END
				0x00,                  // ELEMENT_TYPE_VOID
				PT_Primitive | 0x0004, // ELEMENT_TYPE_BOOLEAN
				PT_Primitive | 0x3F88, // ELEMENT_TYPE_CHAR (W = U2, CHAR, I4, U4, I8, U8, R4, R8) (U2 == Char)
				PT_Primitive | 0x3550, // ELEMENT_TYPE_I1   (W = I1, I2, I4, I8, R4, R8) 
				PT_Primitive | 0x3FE8, // ELEMENT_TYPE_U1   (W = CHAR, U1, I2, U2, I4, U4, I8, U8, R4, R8)
				PT_Primitive | 0x3540, // ELEMENT_TYPE_I2   (W = I2, I4, I8, R4, R8)
				PT_Primitive | 0x3F88, // ELEMENT_TYPE_U2   (W = U2, CHAR, I4, U4, I8, U8, R4, R8)
				PT_Primitive | 0x3500, // ELEMENT_TYPE_I4   (W = I4, I8, R4, R8)
				PT_Primitive | 0x3E00, // ELEMENT_TYPE_U4   (W = U4, I8, R4, R8)
				PT_Primitive | 0x3400, // ELEMENT_TYPE_I8   (W = I8, R4, R8)
				PT_Primitive | 0x3800, // ELEMENT_TYPE_U8   (W = U8, R4, R8)
				PT_Primitive | 0x3000, // ELEMENT_TYPE_R4   (W = R4, R8)
				PT_Primitive | 0x2000, // ELEMENT_TYPE_R8   (W = R8) 
			};
			
			

			public static bool IsPrimitiveType(ElementType type)
			{
				// if (type >= PRIMITIVE_TABLE_SIZE)
				// {
				//     if (ELEMENT_TYPE_I==type || ELEMENT_TYPE_U==type)
				//     {
				//         return TRUE;
				//     }
				//     return 0;
				// }

				// return (PT_Primitive & PrimitiveAttributes[type]);

				if (type >= PRIMITIVE_TABLE_SIZE) {
					if (ElementType.I == type || ElementType.U == type) {
						return true;
					}

					return false;
				}

				return (PT_Primitive & PrimitiveAttributes[(byte) type]) != 0;
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
					var corType         = mt.ElementTypeHandle.NormType;
					var isPrimitiveElem = IsPrimitiveType(corType);

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
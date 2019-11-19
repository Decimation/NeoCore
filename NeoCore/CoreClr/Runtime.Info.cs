#region

using System;
using System.Collections;
using System.Diagnostics;
using System.Runtime;
using System.Runtime.InteropServices;
using InlineIL;
using JetBrains.Annotations;
using NeoCore.CoreClr.Components;
using NeoCore.CoreClr.Components.Support;
using NeoCore.CoreClr.Components.Support.Parsing;
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
		///     Contains utilities for retrieving dynamic information about runtime objects.
		/// </summary>
		public static class Info
		{
			/// <summary>
			///     Hueristically determines whether <paramref name="value" /> is blank.
			///     This always returns <c>true</c> if <paramref name="value" /> is <c>null</c> or nil.
			/// </summary>
			/// <remarks>Blank is defined as one of the following: <c>null</c>, nil (<see cref="IsNil{T}"/>),
			/// non-unique, or unmodified</remarks>
			/// <example>
			///     If <paramref name="value" /> is a <see cref="string" />, this function returns <c>true</c> if the
			///     <see cref="string" /> is <c>null</c> or whitespace (<see cref="string.IsNullOrWhiteSpace"/>).
			/// </example>
			/// <param name="value">Value to check for</param>
			/// <typeparam name="T">Type of <paramref name="value"/></typeparam>
			/// <returns>
			///     <c>true</c> if <paramref name="value" /> is <c>null</c> or nil;
			///     <c>true</c> if <paramref name="value" /> is hueristically determined to be blank.
			/// </returns>
			public static bool IsBlank<T>([CanBeNull] T value)
			{
				if (IsNil(value)) {
					return true;
				}

				if (!Properties.IsStruct(value)) {
					// Null comparison should be equivalent to nil comparison,
					// but this is a sanity check
					if (value == null) {
						return true;
					}
				}
				
				// As for strings, IsNullOrWhiteSpace should always be true when
				// IsNullOrEmpty is true, and vise versa
				
				return value switch
				{
					IList list => (list.Count == 0),
					string str => String.IsNullOrWhiteSpace(str),
					_ => true
				};
			}

			public static bool IsBoxed<T>([CanBeNull] T value)
			{
				return (typeof(T).IsInterface || typeof(T) == typeof(object)) && value != null &&
				       Properties.IsStruct(value);
			}

			[Obsolete]
			public static bool IsPinnableSlow([CanBeNull] object value)
			{
				return !Functions.Inspector.FunctionThrows<ArgumentException>(() =>
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

				if (Properties.IsString(value)) {
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

			/// <summary>
			///     Determines whether the value of <paramref name="value" /> is <c>default</c> or <c>null</c> bytes,
			///     or <paramref name="value" /> is <c>null</c>
			///     <remarks>"Nil" is <c>null</c> or <c>default</c>.</remarks>
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
		}
	}
}
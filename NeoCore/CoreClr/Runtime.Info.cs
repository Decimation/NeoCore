#region

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection.Emit;
using System.Runtime;
using System.Runtime.InteropServices;
using InlineIL;
using JetBrains.Annotations;
using Memkit.Model.Attributes;
using Memkit.Pointers;
using Memkit.Utilities;
using NeoCore.CoreClr.VM;
using NeoCore.Utilities;
using NeoCore.Utilities.Diagnostics;
using NeoCore.Win32;

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
			// https://github.com/dotnet/coreclr/blob/master/src/vm/object.h

			/// <summary>
			/// <list type="bullet">
			///         <item>
			///             <description>+ 2: <see cref="ObjHeader.Padding"/> (x64)</description>
			///         </item>
			///         <item>
			///             <description>+ 2: <see cref="ObjHeader.SyncBlock"/></description>
			///         </item>
			///     </list>
			/// </summary>
			public static readonly unsafe int ObjHeaderSize = sizeof(ObjHeader);

			/// <summary>
			///     Size of <see cref="TypeHandle" /> and <see cref="ObjHeader" />
			///     <list type="bullet">
			///         <item>
			///             <description>+ <see cref="Runtime.Info.ObjHeaderSize" />: <see cref="ObjHeader" /></description>
			///         </item>
			///         <item>
			///             <description>+ sizeof <see cref="TypeHandle" />: <see cref="TypeHandle"/></description>
			///         </item>
			///     </list>
			/// </summary>
			public static readonly unsafe int ObjectBaseSize = ObjHeaderSize + sizeof(TypeHandle);

			/// <summary>
			///     <para>Minimum GC object heap size</para>
			/// </summary>
			public static readonly int MinObjectSize = (Mem.Size * 2) + ObjHeaderSize;

			/// <summary>
			///     Hueristically determines whether <paramref name="value" /> is blank.
			///     This always returns <c>true</c> if <paramref name="value" /> is <c>null</c> or nil.
			/// </summary>
			/// <remarks>Blank is defined as one of the following: <c>null</c>, nil (<see cref="IsNilFast{T}"/>),
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
				if (IsNilFast(value)) {
					return true;
				}

				if (IsBoxed(value)) {
					return false;
				}

				if (!Inspector.IsCompileStruct<T>()) {
					// Null comparison should be equivalent to nil comparison,
					// but this is a sanity check
					if (value == null) {
						return true;
					}
				}

				// As for strings, IsNullOrWhiteSpace should always be true when
				// IsNullOrEmpty is true, and vise versa

				bool? test = value switch
				{
					IList list => (list.Count == 0),
					string str => String.IsNullOrWhiteSpace(str),
					_ => new bool?()
				};

				return !test.HasValue ? Inspector.IsNil(value) : test.Value;
			}

			/// <summary>
			/// Determines whether <paramref name="value"/> is boxed.
			/// </summary>
			/// <param name="value">Value to test for</param>
			/// <typeparam name="T">Type of <paramref name="value"/></typeparam>
			/// <returns><c>true</c> if <paramref name="value"/> is boxed; <c>false</c> otherwise</returns>
			public static bool IsBoxed<T>([CanBeNull] T value)
			{
				return (typeof(T).IsInterface || typeof(T) == typeof(object)) && value != null &&
				       Inspector.IsStruct(value);
			}

			/// <summary>
			/// Determines whether <paramref name="value"/> is pinnable; that is, usable with
			/// <see cref="GCHandle"/> and <see cref="GCHandleType.Pinned"/>
			/// </summary>
			/// <param name="value">Value to check for pinnability</param>
			/// <returns><c>true</c> if <paramref name="value"/> is pinnable; <c>false</c> otherwise</returns>
			public static bool IsPinnable(object? value) => IsPinnable(value, PinTestOptions.Fast);

			/// <summary>
			/// Determines whether <paramref name="value"/> is pinnable; that is, usable with
			/// <see cref="GCHandle"/> and <see cref="GCHandleType.Pinned"/>
			/// </summary>
			/// <param name="value">Value to check for pinnability</param>
			/// <param name="options">How to determine pinnability.</param>
			/// <returns><c>true</c> if <paramref name="value"/> is pinnable; <c>false</c> otherwise</returns>
			private static bool IsPinnable(object? value, PinTestOptions options)
			{
				switch (options) {
					case PinTestOptions.SystemMarshal:
						return Functions.Find<IsPinnableDelegate>()(value);
					case PinTestOptions.GCHandleException:
						return !Functions.Throws<ArgumentException>(() =>
						{
							var gc = GCHandle.Alloc(value, GCHandleType.Pinned);
							gc.Free();
						});
					case PinTestOptions.Fast:
						// https://github.com/dotnet/coreclr/blob/master/src/vm/marshalnative.cpp#L280

						if (value == null) {
							return true;
						}

						var mt = ReadTypeHandle(value);

						if (Inspector.IsString(value)) {
							return true;
						}

						if (mt.IsArray) {
							var  corType         = mt.ElementTypeHandle.NormType;
							bool isPrimitiveElem = Tokens.IsPrimitiveType(corType);

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
					default:
						throw new ArgumentOutOfRangeException(nameof(options), options, null);
				}
			}

			[NativeFunction]
			public static bool IsNilFast<T>([CanBeNull] T value)
			{
				// Fastest method for calculating whether a value is nil.
				IL.Emit.Ldarg(nameof(value));
				IL.Emit.Ldnull();
				IL.Emit.Ceq();
				IL.Emit.Ret();
				return IL.Return<bool>();
			}

			private enum PinTestOptions
			{
				/// <summary>
				/// Determines pinnability using the internal <see cref="Marshal"/> function that tests for pinnability.
				/// </summary>
				SystemMarshal,

				/// <summary>
				/// Determines pinnability by checking whether allocating a pinned <see cref="GCHandle"/>
				/// throws an exception. This is the most reliable method, but it is very slow.
				/// </summary>
				GCHandleException,

				/// <summary>
				/// Determines pinnability by using a function that resembles the internal CLR
				/// source code which tests for pinnability.
				/// </summary>
				Fast
			}

			[FunctionSpecifier(typeof(Marshal))]
			private delegate bool IsPinnableDelegate(object? handle);
		}
	}
}
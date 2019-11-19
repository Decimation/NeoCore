using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime;
using System.Runtime.InteropServices;
using NeoCore.CoreClr.Components.Support;
using NeoCore.CoreClr.Components.VM;
using NeoCore.CoreClr.Meta;
using NeoCore.Interop;
using NeoCore.Memory;
using NeoCore.Memory.Pointers;
using NeoCore.Utilities.Diagnostics;
// ReSharper disable InconsistentNaming

namespace NeoCore.CoreClr
{
	/// <summary>
	/// Contains utilities for interacting with the .NET runtime.
	/// </summary>
	public static unsafe partial class Runtime
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
		
		internal static Type ResolveType(Pointer<byte> handle)
		{
			return Functions.Clr.ReadTypeFromHandle(handle.Address);
		}

		/// <summary>
		/// Returns a pointer to the internal CLR metadata structure of <paramref name="member"/>
		/// </summary>
		/// <param name="member">Reflection type</param>
		/// <returns>A pointer to the corresponding structure</returns>
		/// <exception cref="InvalidOperationException">The type of <see cref="MemberInfo"/> doesn't have a handle</exception>
		internal static Pointer<byte> ResolveHandle(MemberInfo member)
		{
			Guard.AssertNotNull(member, nameof(member));

			return member switch
			{
				Type t => ReadTypeHandle(t).Value.Cast(),
				FieldInfo field => field.FieldHandle.Value,
				MethodInfo method => method.MethodHandle.Value,
				_ => throw new InvalidOperationException()
			};
		}

		public static MetaType ReadTypeHandle<T>(T value)
		{
			// Value types do not have a MethodTable ptr, but they do have a TypeHandle.
			if (Runtime.Properties.IsStruct(value))
				return ReadTypeHandle(value.GetType());

			Unsafe.TryGetAddressOfHeap(value, out Pointer<byte> ptr);
			Guard.AssertNotNull(ptr, nameof(ptr));

			var handle = *(TypeHandle*) ptr;
			return new MetaType(handle.MethodTable);
		}

		public static MetaType ReadTypeHandle(Type t)
		{
			var handle          = t.TypeHandle.Value;
			var typeHandleValue = *(TypeHandle*) &handle;
			return new MetaType(typeHandleValue.MethodTable);
		}

		internal static ObjHeader ReadObjHeader<T>(T value) where T : class
		{
			Pointer<ObjHeader> ptr = Unsafe.AddressOfHeap(value, OffsetOptions.Header).Cast<ObjHeader>();
			return ptr.Value;
		}
	}
}
using System;
using System.Reflection;
using NeoCore.CoreClr.Metadata;
using NeoCore.Interop;
using NeoCore.Memory;
using NeoCore.Utilities.Diagnostics;

namespace NeoCore.CoreClr
{
	/// <summary>
	/// Contains utilities for interacting with the .NET runtime.
	/// </summary>
	public static unsafe partial class Runtime
	{
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
				Type t => ReadTypeHandle(t).MethodTable.Cast(),
				FieldInfo field => field.FieldHandle.Value,
				MethodInfo method => method.MethodHandle.Value,
				_ => throw new InvalidOperationException()
			};
		}
		
		internal static TypeHandle ReadTypeHandle<T>(T value)
		{
			// Value types do not have a MethodTable ptr, but they do have a TypeHandle.
			if (Runtime.Info.IsStruct(value))
				return ReadTypeHandle(value.GetType());

			Unsafe.TryGetAddressOfHeap(value, out Pointer<byte> ptr);
			Guard.AssertNotNull(ptr, nameof(ptr));
			return *(TypeHandle*) ptr;
		}

		internal static TypeHandle ReadTypeHandle(Type t)
		{
			var handle          = t.TypeHandle.Value;
			var typeHandleValue = *(TypeHandle*) &handle;
			return typeHandleValue;
		}
	}
}
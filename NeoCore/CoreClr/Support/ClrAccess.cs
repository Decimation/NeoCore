using System;
using System.Runtime.InteropServices;
using NeoCore.Memory;
using NeoCore.Memory.Pointers;
using NeoCore.Utilities;

namespace NeoCore.CoreClr.Support
{
	public static unsafe class ClrAccess
	{
		// https://github.com/dotnet/coreclr/blob/master/src/inc/daccess.h
		
		/// <summary>
		/// Alias: PTR_HOST_MEMBER_TADDR (alt)
		/// </summary>
		[Obsolete]
		internal static Pointer<byte> FieldOffsetAlt<T>(ref T value, long ofs, Pointer<byte> fieldValue)
			where T : unmanaged
		{
			return Unsafe.AddressOf(ref value).Add((long) fieldValue).Add(ofs).Cast();
		}

		/// <summary>
		/// Alias: PTR_HOST_MEMBER_TADDR
		/// </summary>
		internal static Pointer<byte> FieldOffset<TField>(TField* field, int offset) where TField : unmanaged
		{
			// m_methodTable.GetValue(PTR_HOST_MEMBER_TADDR(MethodDescChunk, this, m_methodTable));

			//const int MT_FIELD_OFS = 0;
			//return (MethodTable*) (MT_FIELD_OFS + ((long) MethodTableRaw));
			
			// // Construct a pointer to a member of the given type.
			// #define PTR_HOST_MEMBER_TADDR(type, host, memb) \
			//     (PTR_HOST_TO_TADDR(host) + (TADDR)offsetof(type, memb))

			return (Pointer<byte>) (offset + ((long) field));
		}
		
		/// <summary>
		/// Alias: PTR_HOST_MEMBER_TADDR
		/// </summary>
		internal static Pointer<byte> FieldOffset<TClr, TField>(TField* field, string name, bool isProperty = false) 
			where TField : unmanaged
		{
			return FieldOffset(field, OffsetOf<TClr>(name, isProperty));
		}

		public static int OffsetOf(Type t, string name, bool isProperty = false)
		{
			if (isProperty) {
				name = Format.GetBackingFieldName(name);
			}

			return (int) Marshal.OffsetOf(t, name);
		}

		public static int OffsetOf<TClr>(string name, bool isProperty = false)
		{
			return OffsetOf(typeof(TClr), name, isProperty);
		}
	}
}
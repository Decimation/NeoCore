using System;
using System.Runtime.InteropServices;
using NeoCore.Memory;
using NeoCore.Utilities;

namespace NeoCore.CoreClr.Support
{
	public static unsafe class ClrAccess
	{
		// https://github.com/dotnet/coreclr/blob/master/src/inc/daccess.h
		
		/// <summary>
		/// Alias: PTR_HOST_MEMBER_TADDR
		/// </summary>
		[Obsolete]
		internal static Pointer<byte> FieldOffset<T>(ref T value, long ofs, Pointer<byte> fieldValue)
			where T : struct
		{
			return Unsafe.AddressOf(ref value).Add((long) fieldValue).Add(ofs).Cast();
		}
		
		public static int OffsetOf(Type t,string name, bool isProperty = false)
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
		
		/// <summary>
		/// Alias: PTR_HOST_TO_TADDR
		/// </summary>
		internal static QInt HostToQInt<TClr>(TClr* host) where TClr : unmanaged
		{
			return (QInt) (host);
		}
		
		internal static Pointer<byte> HostMemberAddress<TClr>(TClr* host, string member, bool isProperty = false) where TClr : unmanaged
		{
			// Construct a pointer to a member of the given type.
			// #define PTR_HOST_MEMBER_TADDR(type, host, memb) \
			// (PTR_HOST_TO_TADDR(host) + (TADDR)offsetof(type, memb))

			var taddr = (ulong) host;
			var ofs = (ulong) OffsetOf<TClr>(member, isProperty);

			// PTR_HOST_MEMBER_TADDR(type, host, memb)
			// Retrieves the target address of a host instance pointer and
			// offsets it by the given member's offset within the type.
			
			// #define PTR_HOST_MEMBER_TADDR(type, host, memb) ((TADDR)&(host)->memb)
			
			return (Pointer<byte>) ((ulong) (taddr + ofs));
		}
		
		// todo: test this version
		internal static Pointer<byte> HostMemberAddress<TClr>(TClr* host, void* member) where TClr : unmanaged
		{
			// Construct a pointer to a member of the given type.
			// #define PTR_HOST_MEMBER_TADDR(type, host, memb) \
			// (PTR_HOST_TO_TADDR(host) + (TADDR)offsetof(type, memb))

			var taddr = HostToQInt(host);
			var ofs = (QInt) ((QInt) host) - ((QInt) member);
			
			
			// PTR_HOST_MEMBER_TADDR(type, host, memb)
			// Retrieves the target address of a host instance pointer and
			// offsets it by the given member's offset within the type.
			
			// #define PTR_HOST_MEMBER_TADDR(type, host, memb) ((TADDR)&(host)->memb)
			
			return (Pointer<byte>) ((ulong) (taddr + ofs));
		}

		/// <summary>
		/// Alias: dac_cast
		/// </summary>
		internal static TTarget Cast<TSource, TTarget>(TSource src)
		{
			// todo:There is no way to determine if we're running a DAC build...
			
			// From coreclr:
			
			// In non-DAC builds, dac_cast is the same as a C-style cast because we need to support:
			//  - casting away const
			//  - conversions between pointers and TADDR
			// Perhaps we should more precisely restrict it's usage, but we get the precise 
			// restrictions in DAC builds, so it wouldn't buy us much.
			return Unsafe.As<TSource, TTarget>(ref src);
		}
	}
}
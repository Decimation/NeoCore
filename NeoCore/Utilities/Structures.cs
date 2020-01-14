using System;
using System.IO;
using System.Runtime.InteropServices;
using Memkit.Model;
using Memkit.Pointers;
using Memkit.Utilities;
using NeoCore.CoreClr.Meta;
using NeoCore.CoreClr.Meta.Base;
using NeoCore.Model;

namespace NeoCore.Utilities
{
	public static class Structures
	{
		// https://github.com/dotnet/coreclr/blob/master/src/inc/daccess.h

		/// <summary>
		/// Alias: PTR_HOST_MEMBER_TADDR (alt)
		/// </summary>
		internal static Pointer<byte> FieldOffsetAlt<T>(ref T value, long ofs, Pointer<byte> fieldValue)
			where T : unmanaged
		{
			return Mem.AddressOf(ref value).Add((long) fieldValue).Add(ofs).Cast();
		}

		/// <summary>
		/// Alias: PTR_HOST_MEMBER_TADDR
		/// </summary>
		internal static unsafe Pointer<byte> FieldOffset<TField>(TField* field, int offset) where TField : unmanaged
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
		internal static unsafe Pointer<byte> FieldOffset<TClr, TField>(TField* field, string name,
		                                                               bool    isProperty = false)
			where TField : unmanaged
		{
			return FieldOffset(field, OffsetOf<TClr>(name, OffsetOfType.Marshal, isProperty));
		}

		internal static Pointer<TSub> ReadSubStructure<TSuper, TSub>(Pointer<TSuper> super)
			where TSub : INativeSubclass<TSuper>
			where TSuper : INativeStructure
		{
			int size = Mem.QuickSizeOf<TSuper>();
			return super.Add(size).Cast<TSub>();
		}

		public static int OffsetOf(Type t, string name, OffsetOfType type, bool isProperty = false)
		{
			if (isProperty) {
				name = EasyReflection.GetBackingFieldName(name);
			}

			switch (type) {
				case OffsetOfType.Marshal:
					return (int) Marshal.OffsetOf(t, name);
				case OffsetOfType.Managed:
					var mt = t.AsMetaType();
					return mt[name].Offset;
				default:
					throw new ArgumentOutOfRangeException(nameof(type), type, null);
			}
		}

		public static int OffsetOf<T>(string name, OffsetOfType type, bool isProperty = false) =>
			OffsetOf(typeof(T), name, type, isProperty);

		
	}

	public enum OffsetOfType
	{
		/// <summary>
		/// <see cref="System.Runtime.InteropServices.Marshal.OffsetOf(Type,String)"/>
		/// </summary>
		Marshal,

		/// <summary>
		/// <see cref="MetaField.Offset"/>
		/// </summary>
		Managed
	}
}
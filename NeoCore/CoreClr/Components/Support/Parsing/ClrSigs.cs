using System;
using NeoCore.Assets;

namespace NeoCore.CoreClr.Components.Support.Parsing
{
	internal static class ClrSigs
	{
		// src/inc/corhdr.h

		private const CorElementType PRIMITIVE_TABLE_SIZE = CorElementType.String;

		private const int PT_PRIMITIVE = 0x01000000;

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
			PT_PRIMITIVE | 0x0004, // ELEMENT_TYPE_BOOLEAN
			PT_PRIMITIVE | 0x3F88, // ELEMENT_TYPE_CHAR (W = U2, CHAR, I4, U4, I8, U8, R4, R8) (U2 == Char)
			PT_PRIMITIVE | 0x3550, // ELEMENT_TYPE_I1   (W = I1, I2, I4, I8, R4, R8) 
			PT_PRIMITIVE | 0x3FE8, // ELEMENT_TYPE_U1   (W = CHAR, U1, I2, U2, I4, U4, I8, U8, R4, R8)
			PT_PRIMITIVE | 0x3540, // ELEMENT_TYPE_I2   (W = I2, I4, I8, R4, R8)
			PT_PRIMITIVE | 0x3F88, // ELEMENT_TYPE_U2   (W = U2, CHAR, I4, U4, I8, U8, R4, R8)
			PT_PRIMITIVE | 0x3500, // ELEMENT_TYPE_I4   (W = I4, I8, R4, R8)
			PT_PRIMITIVE | 0x3E00, // ELEMENT_TYPE_U4   (W = U4, I8, R4, R8)
			PT_PRIMITIVE | 0x3400, // ELEMENT_TYPE_I8   (W = I8, R4, R8)
			PT_PRIMITIVE | 0x3800, // ELEMENT_TYPE_U8   (W = U8, R4, R8)
			PT_PRIMITIVE | 0x3000, // ELEMENT_TYPE_R4   (W = R4, R8)
			PT_PRIMITIVE | 0x2000, // ELEMENT_TYPE_R8   (W = R8) 
		};


		public static bool IsPrimitiveType(CorElementType type)
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
				return CorElementType.I == type || CorElementType.U == type;
			}

			return (PT_PRIMITIVE & PrimitiveAttributes[(byte) type]) != 0;
		}

		internal static bool IsNilToken(int tk) => RidFromToken(tk) == 0;

		internal static int RidToToken(int rid, CorTokenType tktype)
		{
			// #define RidToToken(rid,tktype) ((rid) |= (tktype))
			(rid) |= ((int) tktype);
			return rid;
		}

		internal static int TokenFromRid(int rid, CorTokenType tktype)
		{
			// #define TokenFromRid(rid,tktype) ((rid) | (tktype))
			return rid | (int) tktype;
		}

		internal static int RidFromToken(int tk)
		{
			// #define RidFromToken(tk) ((RID) ((tk) & 0x00ffffff))
			const int RID_FROM_TOKEN = 0x00FFFFFF;
			return tk & RID_FROM_TOKEN;
		}

		internal static long TypeFromToken(int tk)
		{
			// #define TypeFromToken(tk) ((ULONG32)((tk) & 0xff000000))

			const uint TYPE_FROM_TOKEN = 0xFF000000;
			return tk & TYPE_FROM_TOKEN;
		}

		public static bool IsPrimitive(this CorElementType cet)
		{
			return cet >= CorElementType.Boolean && cet <= CorElementType.R8
			       || cet == CorElementType.I || cet == CorElementType.U
			       || cet == CorElementType.Ptr || cet == CorElementType.FnPtr;
		}

		/// <summary>
		///     <exception cref="Exception">If size is unknown</exception>
		/// </summary>
		internal static int SizeOfElementType(CorElementType t)
		{
			switch (t) {
				case CorElementType.Void:
					return default;

				case CorElementType.Boolean:
					return sizeof(bool);

				case CorElementType.Char:
					return sizeof(char);

				case CorElementType.I1:
					return sizeof(sbyte);
				case CorElementType.U1:
					return sizeof(byte);

				case CorElementType.I2:
					return sizeof(short);
				case CorElementType.U2:
					return sizeof(ushort);

				case CorElementType.I4:
					return sizeof(int);
				case CorElementType.U4:
					return sizeof(uint);

				case CorElementType.I8:
					return sizeof(long);
				case CorElementType.U8:
					return sizeof(ulong);

				case CorElementType.R4:
					return sizeof(float);
				case CorElementType.R8:
					return sizeof(double);

				case CorElementType.String:
				case CorElementType.Ptr:
				case CorElementType.ByRef:
				case CorElementType.Class:
				case CorElementType.Array:
				case CorElementType.I:
				case CorElementType.U:
				case CorElementType.FnPtr:
				case CorElementType.Object:
				case CorElementType.SzArray:
				case CorElementType.End:
					return IntPtr.Size;


				case CorElementType.ValueType:
				case CorElementType.Var:
				case CorElementType.GenericInst:
				case CorElementType.CModReqd:
				case CorElementType.CModOpt:
				case CorElementType.Internal:
				case CorElementType.MVar:
					return Constants.INVALID_VALUE;

				case CorElementType.TypedByRef:
					return IntPtr.Size * 2;

				case CorElementType.Max:
				case CorElementType.Modifier:
				case CorElementType.Sentinel:
				case CorElementType.Pinned:
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(t), t, null);
			}

			throw new InvalidOperationException();
		}
	}
}
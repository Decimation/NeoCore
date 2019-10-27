using System;
using NeoCore.Assets;
using NeoCore.CoreClr.Components.VM;

namespace NeoCore.CoreClr.Components.Support
{
	internal static class ClrSigs
	{
		// src/inc/corhdr.h

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

		internal static bool IsNilToken(int tk)
		{
			return RidFromToken(tk) == 0;
		}

		internal static int RidToToken(int rid, TokenType tktype)
		{
			// #define RidToToken(rid,tktype) ((rid) |= (tktype))
			(rid) |= ((int) tktype);
			return rid;
		}

		internal static int TokenFromRid(int rid, TokenType tktype)
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
		
		public static bool IsPrimitive(this ElementType cet)
		{
			return cet >= ElementType.Boolean && cet <= ElementType.R8
			       || cet == ElementType.I || cet == ElementType.U
			       || cet == ElementType.Ptr || cet == ElementType.FnPtr;
		}

		/// <summary>
		///     <exception cref="Exception">If size is unknown</exception>
		/// </summary>
		internal static int SizeOfElementType(ElementType t)
		{
			switch (t) {
				case ElementType.Void:
					return default;

				case ElementType.Boolean:
					return sizeof(bool);

				case ElementType.Char:
					return sizeof(char);

				case ElementType.I1:
					return sizeof(sbyte);
				case ElementType.U1:
					return sizeof(byte);

				case ElementType.I2:
					return sizeof(short);
				case ElementType.U2:
					return sizeof(ushort);

				case ElementType.I4:
					return sizeof(int);
				case ElementType.U4:
					return sizeof(uint);

				case ElementType.I8:
					return sizeof(long);
				case ElementType.U8:
					return sizeof(ulong);

				case ElementType.R4:
					return sizeof(float);
				case ElementType.R8:
					return sizeof(double);

				case ElementType.String:
				case ElementType.Ptr:
				case ElementType.ByRef:
				case ElementType.Class:
				case ElementType.Array:
				case ElementType.I:
				case ElementType.U:
				case ElementType.FnPtr:
				case ElementType.Object:
				case ElementType.SzArray:
				case ElementType.End:
					return IntPtr.Size;


				case ElementType.ValueType:
				case ElementType.Var:
				case ElementType.GenericInst:
				case ElementType.CModReqd:
				case ElementType.CModOpt:
				case ElementType.Internal:
				case ElementType.MVar:
					return Constants.INVALID_VALUE;

				case ElementType.TypedByRef:
					return IntPtr.Size * 2;

				case ElementType.Max:
				case ElementType.Modifier:
				case ElementType.Sentinel:
				case ElementType.Pinned:
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(t), t, null);
			}

			throw new InvalidOperationException();
		}
	}
}
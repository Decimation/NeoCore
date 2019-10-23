using System;
using NeoCore.Assets;
using NeoCore.CoreClr.Metadata;

namespace NeoCore.CoreClr
{
	internal static class Tokens
	{
		private const int RID_FROM_TOKEN = 0x00FFFFFF;

		private const uint TYPE_FROM_TOKEN = 0xFF000000;

		internal static int TokenFromRid(int rid, TokenType tktype) => rid | (int) tktype;

		internal static int RidFromToken(int tk) => tk & RID_FROM_TOKEN;

		internal static long TypeFromToken(int tk) => tk & TYPE_FROM_TOKEN;

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
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace NeoCore.CoreClr.Components.Support
{
	internal struct SigParser
	{
		// https://github.com/microsoft/clrmd/blob/master/src/Microsoft.Diagnostics.Runtime/src/Common/ClrElementTypeExtensions.cs
		// https://github.com/dotnet/coreclr/blob/master/src/inc/sigparser.h
		// https://github.com/microsoft/clrmd/blob/master/src/Microsoft.Diagnostics.Runtime/src/Utilities/SigParser/SigParser.cs
		
		private byte[] _sig;
		private int    _len;
		private int    _offs;

		public SigParser(byte[] sig, int len)
		{
			_sig  = sig;
			_len  = len;
			_offs = 0;
		}

		public SigParser(SigParser rhs)
		{
			_sig  = rhs._sig;
			_len  = rhs._len;
			_offs = rhs._offs;
		}

		public SigParser(IntPtr sig, int len)
		{
			if (len != 0) {
				_sig = new byte[len];
				Marshal.Copy(sig, _sig, 0, _sig.Length);
			}
			else {
				_sig = null;
			}

			_len  = len;
			_offs = 0;
		}

		public bool IsNull()
		{
			return _sig == null;
		}

		private void CopyFrom(SigParser rhs)
		{
			_sig  = rhs._sig;
			_len  = rhs._len;
			_offs = rhs._offs;
		}

		private void SkipBytes(int bytes)
		{
			Debug.Assert(bytes <= _len);
			_offs += bytes;
			_len  -= bytes;
			Debug.Assert(_len <= 0 || _offs < _sig.Length);
		}

		private bool SkipInt()
		{
			return GetData(out int tmp);
		}

		public bool GetData(out int data)
		{
			if (UncompressData(out data, out int size)) {
				SkipBytes(size);
				return true;
			}

			return false;
		}

		private bool GetByte(out byte data)
		{
			if (_len <= 0) {
				data = 0xcc;
				return false;
			}

			data = _sig[_offs];
			SkipBytes(1);
			return true;
		}

		private bool PeekByte(out byte data)
		{
			if (_len <= 0) {
				data = 0xcc;
				return false;
			}

			data = _sig[_offs];
			return true;
		}

		private bool GetElemTypeSlow(out int etype)
		{
			SigParser sigTemp = new SigParser(this);
			if (sigTemp.SkipCustomModifiers()) {
				if (sigTemp.GetByte(out byte elemType)) {
					etype = elemType;
					CopyFrom(sigTemp);
					return true;
				}
			}

			etype = 0;
			return false;
		}

		public bool GetElemType(out int etype)
		{
			if (_len > 0) {
				byte type = _sig[_offs];

				if (type < ELEMENT_TYPE_CMOD_REQD) // fast path with no modifiers: single byte
				{
					etype = type;
					SkipBytes(1);
					return true;
				}
			}

			// Slower/normal path
			return GetElemTypeSlow(out etype);
		}

		public bool PeekCallingConvInfo(out int data)
		{
			return PeekByte(out data);
		}

		// Note: Calling convention is always one byte, not four bytes
		public bool GetCallingConvInfo(out int data)
		{
			if (PeekByte(out data)) {
				SkipBytes(1);
				return true;
			}

			return false;
		}

		private bool GetCallingConv(out int data)
		{
			if (GetCallingConvInfo(out data)) {
				data &= IMAGE_CEE_CS_CALLCONV_MASK;
				return true;
			}

			return false;
		}

		private bool PeekData(out int data)
		{
			return UncompressData(out data, out int size);
		}

		private bool PeekElemTypeSlow(out int etype)
		{
			SigParser sigTemp = new SigParser(this);
			return sigTemp.GetElemType(out etype);
		}

		public bool PeekElemType(out int etype)
		{
			if (_len > 0) {
				byte type = _sig[_offs];
				if (type < ELEMENT_TYPE_CMOD_REQD) {
					etype = type;
					return true;
				}
			}

			return PeekElemTypeSlow(out etype);
		}

		private bool PeekElemTypeSize(out int pSize)
		{
			pSize = 0;
			SigParser sigTemp = new SigParser(this);

			if (!sigTemp.SkipAnyVASentinel())
				return false;

			if (!sigTemp.GetByte(out byte bElementType))
				return false;

			switch ((ElementType)bElementType) {
				case ElementType.I8:
				case ElementType.U8:
				case ElementType.R8:
					pSize = 8;
					break;

				case ElementType.I4:
				case ElementType.U4:
				case ElementType.R4:
					pSize = 4;
					break;

				case ElementType.I2:
				case ElementType.U2:
				case ElementType.Char:
					pSize = 2;
					break;

				case ElementType.I1:
				case ElementType.U1:
				case ElementType.Boolean:
					pSize = 1;
					break;

				case ElementType.I:
				case ElementType.U:
				case ElementType.String:
				case ElementType.Ptr:
				case ElementType.ByRef:
				case ElementType.Class:
				case ElementType.Object:
				case ElementType.FnPtr:
				case ElementType.TypedByRef:
				case ElementType.Array:
				case ElementType.SzArray:
					pSize = IntPtr.Size;
					break;

				case ElementType.Void:
					break;

				case ElementType.End:
				case ElementType.CModReqd:
				case ElementType.CModOpt:
				case ElementType.ValueType:
					Debug.Fail("Asked for the size of an element that doesn't have a size!");
					return false;

				default:
					Debug.Fail("CorSigGetElementTypeSize given invalid signature to size!");
					return false;
			}

			return true;
		}

		private bool AtSentinel()
		{
			if (_len > 0)
				return _sig[_offs] == (int) ElementType.Sentinel;

			return false;
		}

		public bool GetToken(out int token)
		{
			if (UncompressToken(out token, out int size)) {
				SkipBytes(size);
				return true;
			}

			return false;
		}

		public bool SkipCustomModifiers()
		{
			SigParser sigTemp = new SigParser(this);

			if (!sigTemp.SkipAnyVASentinel())
				return false;

			if (!sigTemp.PeekByte(out byte bElementType))
				return false;

			while (ELEMENT_TYPE_CMOD_REQD == bElementType || ELEMENT_TYPE_CMOD_OPT == bElementType) {
				sigTemp.SkipBytes(1);

				if (!sigTemp.GetToken(out int token))
					return false;

				if (!sigTemp.PeekByte(out bElementType))
					return false;
			}

			// Following custom modifiers must be an element type value which is less than ELEMENT_TYPE_MAX, or one of the other element types
			// that we support while parsing various signatures
			if (bElementType >= ELEMENT_TYPE_MAX) {
				switch (bElementType) {
					case ELEMENT_TYPE_PINNED:
						break;
					default:
						return false;
				}
			}

			CopyFrom(sigTemp);
			return true;
		}

		private bool SkipFunkyAndCustomModifiers()
		{
			SigParser sigTemp = new SigParser(this);
			if (!sigTemp.SkipAnyVASentinel())
				return false;

			if (!sigTemp.PeekByte(out byte bElementType))
				return false;

			while (ELEMENT_TYPE_CMOD_REQD == bElementType ||
			       ELEMENT_TYPE_CMOD_OPT == bElementType ||
			       ELEMENT_TYPE_MODIFIER == bElementType ||
			       ELEMENT_TYPE_PINNED == bElementType) {
				sigTemp.SkipBytes(1);

				if (!sigTemp.GetToken(out int token))
					return false;

				if (!sigTemp.PeekByte(out bElementType))
					return false;
			}

			// Following custom modifiers must be an element type value which is less than ELEMENT_TYPE_MAX, or one of the other element types
			// that we support while parsing various signatures
			if (bElementType >= ELEMENT_TYPE_MAX) {
				switch (bElementType) {
					case ELEMENT_TYPE_PINNED:
						break;
					default:
						return false;
				}
			}

			CopyFrom(sigTemp);
			return true;
		} // SkipFunkyAndCustomModifiers

		private bool SkipAnyVASentinel()
		{
			if (!PeekByte(out byte bElementType))
				return false;

			if (bElementType == ELEMENT_TYPE_SENTINEL)
				SkipBytes(1);

			return true;
		} // SkipAnyVASentinel

		public bool SkipExactlyOne()
		{
			if (!GetElemType(out int typ))
				return false;

			if (!((ElementType) typ).IsPrimitive()) {
				switch ((ElementType)typ) {
					default:
						return false;

					case ElementType.Var:
					case ElementType.MVar:
						if (!GetData(out _))
							return false;

						break;

					case ElementType.Object:
					case ElementType.String:
					case ElementType.TypedByRef:
						break;

					case ElementType.ByRef:
					case ElementType.Ptr:
					case ElementType.Pinned:
					case ElementType.SzArray:
						if (!SkipExactlyOne())
							return false;

						break;

					case ElementType.ValueType:
					case ElementType.Class:
						if (!GetToken(out _))
							return false;

						break;

					case ElementType.FnPtr:
						if (!SkipSignature())
							return false;

						break;

					case ElementType.Array:
						// Skip element type
						if (!SkipExactlyOne())
							return false;

						// Get rank;
						int rank;
						if (!GetData(out rank))
							return false;

						if (rank > 0) {
							if (!GetData(out int sizes))
								return false;

							while (sizes-- != 0)
								if (!GetData(out _))
									return false;

							if (!GetData(out int bounds))
								return false;

							while (bounds-- != 0)
								if (!GetData(out _))
									return false;
						}

						break;

					case ElementType.Sentinel:
						// Should be unreachable since GetElem strips it
						break;

					case ElementType.Internal:
						if (!GetData(out _))
							return false;

						break;

					case ElementType.GenericInst:
						// Skip generic type
						if (!SkipExactlyOne())
							return false;

						// Get number of parameters
						int argCnt;
						if (!GetData(out argCnt))
							return false;

						// Skip the parameters
						while (argCnt-- != 0)
							SkipExactlyOne();
						break;
				}
			}

			return true;
		}

		private bool SkipMethodHeaderSignature(out int pcArgs)
		{
			pcArgs = 0;

			// Skip calling convention
			if (!GetCallingConvInfo(out int uCallConv))
				return false;

			if (uCallConv == IMAGE_CEE_CS_CALLCONV_FIELD || uCallConv == IMAGE_CEE_CS_CALLCONV_LOCAL_SIG)
				return false;

			// Skip type parameter count
			if ((uCallConv & IMAGE_CEE_CS_CALLCONV_GENERIC) == IMAGE_CEE_CS_CALLCONV_GENERIC)
				if (!GetData(out _))
					return false;

			// Get arg count;
			if (!GetData(out pcArgs))
				return false;

			// Skip return type;
			if (!SkipExactlyOne())
				return false;

			return true;
		} // SigParser::SkipMethodHeaderSignature

		private bool SkipSignature()
		{
			if (!SkipMethodHeaderSignature(out int args))
				return false;

			// Skip args.
			while (args-- > 0)
				if (!SkipExactlyOne())
					return false;

			return false;
		}

		private bool UncompressToken(out int token, out int size)
		{
			if (!UncompressData(out token, out size))
				return false;

			int tkType = CorEncodeToken[token & 3];
			token = (token >> 2) | tkType;
			return true;
		}

		private byte GetSig(int offs)
		{
			Debug.Assert(offs < _len);
			return _sig[_offs + offs];
		}

		private bool UncompressData(out int pDataOut, out int pDataLen)
		{
			pDataOut = 0;
			pDataLen = 0;

			if (_len <= 0)
				return false;

			byte byte0 = GetSig(0);

			// Smallest.
			if ((byte0 & 0x80) == 0x00) // 0??? ????
			{
				if (_len < 1) {
					return false;
				}

				pDataOut = byte0;
				pDataLen = 1;
			}
			// Medium.
			else if ((byte0 & 0xC0) == 0x80) // 10?? ????
			{
				if (_len < 2) {
					return false;
				}

				pDataOut = ((byte0 & 0x3f) << 8) | GetSig(1);
				pDataLen = 2;
			}
			else if ((byte0 & 0xE0) == 0xC0) // 110? ????
			{
				if (_len < 4) {
					return false;
				}

				pDataOut = ((byte0 & 0x1f) << 24) | (GetSig(1) << 16) | (GetSig(2) << 8) | GetSig(3);
				pDataLen = 4;
			}
			else // We don't recognize this encoding
			{
				return false;
			}

			return true;
		}

		private bool PeekByte(out int data)
		{
			if (!PeekByte(out byte tmp)) {
				data = 0xcc;
				return false;
			}

			data = tmp;
			return true;
		}

		// todo: switch to enums

		private const int MDT_MODULE                   = 0x00000000; //
		private const int MDT_TYPE_REF                 = 0x01000000; //
		private const int MDT_TYPE_DEF                 = 0x02000000; //
		private const int MDT_FIELD_DEF                = 0x04000000; //
		private const int MDT_METHOD_DEF               = 0x06000000; //
		private const int MDT_PARAM_DEF                = 0x08000000; //
		private const int MDT_INTERFACE_IMPL           = 0x09000000; //
		private const int MDT_MEMBER_REF               = 0x0a000000; //
		private const int MDT_CUSTOM_ATTRIBUTE         = 0x0c000000; //
		private const int MDT_PERMISSION               = 0x0e000000; //
		private const int MDT_SIGNATURE                = 0x11000000; //
		private const int MDT_EVENT                    = 0x14000000; //
		private const int MDT_PROPERTY                 = 0x17000000; //
		private const int MDT_METHOD_IMPL              = 0x19000000; //
		private const int MDT_MODULE_REF               = 0x1a000000; //
		private const int MDT_TYPE_SPEC                = 0x1b000000; //
		private const int MDT_ASSEMBLY                 = 0x20000000; //
		private const int MDT_ASSEMBLY_REF             = 0x23000000; //
		private const int MDT_FILE                     = 0x26000000; //
		private const int MDT_EXPORTED_TYPE            = 0x27000000; //
		private const int MDT_MANIFEST_RESOURCE        = 0x28000000; //
		private const int MDT_GENERIC_PARAM            = 0x2a000000; //
		private const int MDT_METHOD_SPEC              = 0x2b000000; //
		private const int MDT_GENERIC_PARAM_CONSTRAINT = 0x2c000000;
		private const int MDT_STRING = 0x70000000; //
		private const int MDT_NAME   = 0x71000000; //

		private const int
			MDT_BASE_TYPE = 0x72000000; // Leave this on the high end value. This does not correspond to metadata table

		private static readonly int[] CorEncodeToken = {MDT_TYPE_DEF, MDT_TYPE_REF, MDT_TYPE_SPEC, MDT_BASE_TYPE};

		private const int IMAGE_CEE_CS_CALLCONV_DEFAULT = 0x0;

		public const int IMAGE_CEE_CS_CALLCONV_VARARG       = 0x5;
		public const int IMAGE_CEE_CS_CALLCONV_FIELD        = 0x6;
		public const int IMAGE_CEE_CS_CALLCONV_LOCAL_SIG    = 0x7;
		public const int IMAGE_CEE_CS_CALLCONV_PROPERTY     = 0x8;
		public const int IMAGE_CEE_CS_CALLCONV_UNMGD        = 0x9;
		public const int IMAGE_CEE_CS_CALLCONV_GENERICINST  = 0xa;
		public const int IMAGE_CEE_CS_CALLCONV_NATIVEVARARG = 0xb;
		public const int IMAGE_CEE_CS_CALLCONV_MAX          = 0xc;

		public const int IMAGE_CEE_CS_CALLCONV_MASK         = 0x0f;
		public const int IMAGE_CEE_CS_CALLCONV_HASTHIS      = 0x20;
		public const int IMAGE_CEE_CS_CALLCONV_EXPLICITTHIS = 0x40;
		public const int IMAGE_CEE_CS_CALLCONV_GENERIC      = 0x10;

		private const int ELEMENT_TYPE_END     = 0x0;
		private const int ELEMENT_TYPE_VOID    = 0x1;
		private const int ELEMENT_TYPE_BOOLEAN = 0x2;
		private const int ELEMENT_TYPE_CHAR    = 0x3;
		private const int ELEMENT_TYPE_I1      = 0x4;
		private const int ELEMENT_TYPE_U1      = 0x5;
		private const int ELEMENT_TYPE_I2      = 0x6;
		private const int ELEMENT_TYPE_U2      = 0x7;
		private const int ELEMENT_TYPE_I4      = 0x8;
		private const int ELEMENT_TYPE_U4      = 0x9;
		private const int ELEMENT_TYPE_I8      = 0xa;
		private const int ELEMENT_TYPE_U8      = 0xb;
		private const int ELEMENT_TYPE_R4      = 0xc;
		private const int ELEMENT_TYPE_R8      = 0xd;
		private const int ELEMENT_TYPE_STRING  = 0xe;

		private const int ELEMENT_TYPE_PTR   = 0xf;
		private const int ELEMENT_TYPE_BYREF = 0x10;

		private const int ELEMENT_TYPE_VALUETYPE   = 0x11;
		private const int ELEMENT_TYPE_CLASS       = 0x12;
		private const int ELEMENT_TYPE_VAR         = 0x13;
		private const int ELEMENT_TYPE_ARRAY       = 0x14;
		private const int ELEMENT_TYPE_GENERICINST = 0x15;
		private const int ELEMENT_TYPE_TYPEDBYREF  = 0x16;

		private const int ELEMENT_TYPE_I       = 0x18;
		private const int ELEMENT_TYPE_U       = 0x19;
		private const int ELEMENT_TYPE_FNPTR   = 0x1B;
		private const int ELEMENT_TYPE_OBJECT  = 0x1C;
		private const int ELEMENT_TYPE_SZARRAY = 0x1D;
		private const int ELEMENT_TYPE_MVAR    = 0x1e;

		private const int ELEMENT_TYPE_CMOD_REQD = 0x1F;
		private const int ELEMENT_TYPE_CMOD_OPT  = 0x20;

		private const int ELEMENT_TYPE_INTERNAL = 0x21;
		private const int ELEMENT_TYPE_MAX      = 0x22;

		private const int ELEMENT_TYPE_MODIFIER = 0x40;
		private const int ELEMENT_TYPE_SENTINEL = 0x01 | ELEMENT_TYPE_MODIFIER;
		private const int ELEMENT_TYPE_PINNED   = 0x05 | ELEMENT_TYPE_MODIFIER;
	}
}
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

		internal SigParser(byte[] sig, int len)
		{
			_sig  = sig;
			_len  = len;
			_offs = 0;
		}

		private SigParser(SigParser rhs)
		{
			_sig  = rhs._sig;
			_len  = rhs._len;
			_offs = rhs._offs;
		}

		internal SigParser(IntPtr sig, int len)
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

		private SigParser Copy() => new SigParser(this);

		public bool IsNull() => _sig == null;

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

		private bool SkipInt() => GetData(out _);

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

		private bool GetElemTypeSlow(out CorElementType etype)
		{
			var sigTemp = Copy();

			if (sigTemp.SkipCustomModifiers()) {
				if (sigTemp.GetByte(out byte elemType)) {
					etype = (CorElementType) elemType;
					CopyFrom(sigTemp);
					return true;
				}
			}

			etype = 0;
			return false;
		}

		public bool GetElemType(out CorElementType etype)
		{
			if (_len > 0) {
				byte type = _sig[_offs];

				if (type < (int) CorElementType.CModReqd) // fast path with no modifiers: single byte
				{
					etype = (CorElementType) type;
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
				data &= (int) CorCallingConvention.MASK;
				return true;
			}

			return false;
		}

		private bool PeekData(out int data)
		{
			return UncompressData(out data, out int size);
		}

		private bool PeekElemTypeSlow(out CorElementType etype)
		{
			var sigTemp = Copy();
			return sigTemp.GetElemType(out etype);
		}

		public bool PeekElemType(out CorElementType etype)
		{
			if (_len > 0) {
				byte type = _sig[_offs];
				if (type < (int) CorElementType.CModReqd) {
					etype = (CorElementType) type;
					return true;
				}
			}

			return PeekElemTypeSlow(out etype);
		}

		private bool PeekElemTypeSize(out int pSize)
		{
			pSize = 0;
			var sigTemp = Copy();

			if (!sigTemp.SkipAnyVASentinel())
				return false;

			if (!sigTemp.GetByte(out byte bElementType))
				return false;

			switch ((CorElementType) bElementType) {
				case CorElementType.I8:
				case CorElementType.U8:
				case CorElementType.R8:
					pSize = 8;
					break;

				case CorElementType.I4:
				case CorElementType.U4:
				case CorElementType.R4:
					pSize = 4;
					break;

				case CorElementType.I2:
				case CorElementType.U2:
				case CorElementType.Char:
					pSize = 2;
					break;

				case CorElementType.I1:
				case CorElementType.U1:
				case CorElementType.Boolean:
					pSize = 1;
					break;

				case CorElementType.I:
				case CorElementType.U:
				case CorElementType.String:
				case CorElementType.Ptr:
				case CorElementType.ByRef:
				case CorElementType.Class:
				case CorElementType.Object:
				case CorElementType.FnPtr:
				case CorElementType.TypedByRef:
				case CorElementType.Array:
				case CorElementType.SzArray:
					pSize = IntPtr.Size;
					break;

				case CorElementType.Void:
					break;

				case CorElementType.End:
				case CorElementType.CModReqd:
				case CorElementType.CModOpt:
				case CorElementType.ValueType:
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
				return _sig[_offs] == (int) CorElementType.Sentinel;

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
			var sigTemp = Copy();

			if (!sigTemp.SkipAnyVASentinel())
				return false;

			if (!sigTemp.PeekByte(out byte bElementType))
				return false;

			while ((int) CorElementType.CModReqd == bElementType || (int) CorElementType.CModOpt == bElementType) {
				sigTemp.SkipBytes(1);

				if (!sigTemp.GetToken(out int token))
					return false;

				if (!sigTemp.PeekByte(out bElementType))
					return false;
			}

			// Following custom modifiers must be an element type value which is less than (int) ElementType.Max, or one of the other element types
			// that we support while parsing various signatures
			if (bElementType >= (int) CorElementType.Max) {
				switch (bElementType) {
					case (int) CorElementType.Pinned:
						break;
					default:
						return false;
				}
			}

			CopyFrom(sigTemp);
			return true;
		}

		private bool SkipAbnormalAndCustomModifiers()
		{
			var sigTemp = Copy();
			if (!sigTemp.SkipAnyVASentinel())
				return false;

			if (!sigTemp.PeekByte(out byte bElementType))
				return false;

			while ((int) CorElementType.CModReqd == bElementType ||
			       (int) CorElementType.CModOpt == bElementType ||
			       (int) CorElementType.Modifier == bElementType ||
			       (int) CorElementType.Pinned == bElementType) {
				sigTemp.SkipBytes(1);

				if (!sigTemp.GetToken(out int token))
					return false;

				if (!sigTemp.PeekByte(out bElementType))
					return false;
			}

			// Following custom modifiers must be an element type value which is less than (int) ElementType.Max, or one of the other element types
			// that we support while parsing various signatures
			if (bElementType >= (int) CorElementType.Max) {
				switch (bElementType) {
					case (int) CorElementType.Pinned:
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

			if (bElementType == (int) CorElementType.Sentinel)
				SkipBytes(1);

			return true;
		} // SkipAnyVASentinel

		public bool SkipExactlyOne()
		{
			if (!GetElemType(out var typ))
				return false;

			if (!typ.IsPrimitive()) {
				switch (typ) {
					default:
						return false;

					case CorElementType.Var:
					case CorElementType.MVar:
						if (!GetData(out _))
							return false;

						break;

					case CorElementType.Object:
					case CorElementType.String:
					case CorElementType.TypedByRef:
						break;

					case CorElementType.ByRef:
					case CorElementType.Ptr:
					case CorElementType.Pinned:
					case CorElementType.SzArray:
						if (!SkipExactlyOne())
							return false;

						break;

					case CorElementType.ValueType:
					case CorElementType.Class:
						if (!GetToken(out _))
							return false;

						break;

					case CorElementType.FnPtr:
						if (!SkipSignature())
							return false;

						break;

					case CorElementType.Array:
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

					case CorElementType.Sentinel:
						// Should be unreachable since GetElem strips it
						break;

					case CorElementType.Internal:
						if (!GetData(out _))
							return false;

						break;

					case CorElementType.GenericInst:
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

			if (uCallConv == (int) CorCallingConvention.FIELD || uCallConv == (int) CorCallingConvention.LOCAL_SIG)
				return false;

			// Skip type parameter count
			if ((uCallConv & (int) CorCallingConvention.GENERIC) == (int) CorCallingConvention.GENERIC)
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

		private static readonly int[] CorEncodeToken =
		{
			(int) CorTokenType.TypeDef,
			(int) CorTokenType.TypeRef,
			(int) CorTokenType.TypeSpec,
			(int) CorTokenType.BaseType,
		};
	}
}
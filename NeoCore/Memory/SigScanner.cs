using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using NeoCore.Memory.Pointers;

namespace NeoCore.Memory
{
	/// <summary>
	///     A basic sig scanner.
	/// </summary>
	public class SigScanner
	{
		private readonly byte[]        m_buffer;
		private readonly Pointer<byte> m_lo;


		public SigScanner(Region r)
		{
			m_buffer = r.Low.Copy((int) r.Size);
			m_lo     = r.Low;
		}

		private bool PatternCheck(int nOffset, IReadOnlyList<byte> arrPattern)
		{
			// ReSharper disable once LoopCanBeConvertedToQuery
			for (int i = 0; i < arrPattern.Count; i++) {
				if (arrPattern[i] == 0x0)
					continue;

				if (arrPattern[i] != m_buffer[nOffset + i])
					return false;
			}

			return true;
		}

		public static byte[] ParseByteArray(string szPattern)
		{
//			List<byte> patternbytes = new List<byte>();
//			foreach (string szByte in szPattern.Split(' '))
//				patternbytes.Add(szByte == "?" ? (byte) 0x0 : Convert.ToByte(szByte, 16));
//			return patternbytes.ToArray();


			string[] strByteArr   = szPattern.Split(' ');
			var      patternBytes = new byte[strByteArr.Length];

			for (int i = 0; i < strByteArr.Length; i++) {
				patternBytes[i] = strByteArr[i] == "?"
					? (byte) 0x0
					: Byte.Parse(strByteArr[i], NumberStyles.HexNumber);
			}


			return patternBytes;
		}

		public Pointer<byte> FindPattern(string pattern)
		{
			return FindPattern(ParseByteArray(pattern));
		}

		public Pointer<byte> FindPattern(byte[] pattern)
		{
			for (int i = 0; i < m_buffer.Length; i++) {
				if (m_buffer[i] != pattern[0])
					continue;


				if (PatternCheck(i, pattern)) {
					Pointer<byte> p = m_lo + i;
					return p;
				}
			}

			return Mem.Nullptr;
		}
	}
}
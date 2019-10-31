using System.IO;
using System.Runtime.InteropServices;
using NeoCore.Memory;

namespace NeoCore.Utilities.Extensions
{
	public static class BinaryReaderExtensions
	{
		public static T ReadStructure<T>(this BinaryReader reader) where T : struct
		{
			// Read in a byte array
			byte[] bytes = reader.ReadBytes(Marshal.SizeOf<T>());

			return Mem.ReadStructure<T>(bytes);
		}
	}
}
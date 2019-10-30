using System;
using NeoCore.Interop.Enums;
using NeoCore.Interop.Structures.Raw;
using NeoCore.Memory.Pointers;

namespace NeoCore.Interop.Structures
{
	/// <summary>
	/// Wraps an <see cref="ImageSectionHeader"/>
	/// </summary>
	public sealed class ImageSectionInfo
	{
		public string Name { get; }

		public int Number { get; }

		public Pointer<byte> Address { get; }

		public int Size { get; }

		public ImageSectionFlags Characteristics { get; }

		internal ImageSectionInfo(ImageSectionHeader struc, int number, IntPtr address)
		{
			Number          = number;
			Name            = new string(struc.Name);
			Address         = address;
			Size            = (int) struc.VirtualSize;
			Characteristics = struc.Characteristics;
		}

		public override string ToString()
		{
			return String.Format("Number: {0} | Name: {1} | Address: {2} | Size: {3} | Characteristics: {4}", 
			                     Number, Name, Address, Size, Characteristics);
		}
	}
}
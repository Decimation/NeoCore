#region

using System;
using System.Runtime.InteropServices;
using NeoCore.Interop.Attributes;
using NeoCore.Interop.Structures.Raw.Enums;

#endregion

namespace NeoCore.Interop.Structures.Raw
{
	/// <summary>
	///     Contains information about a range of pages in the virtual address space of a process.
	/// </summary>
	[NativeStructure]
	[StructLayout(LayoutKind.Sequential)]
	internal struct MemoryBasicInformation
	{
		/// <summary>
		///     A pointer to the base address of the region of pages.
		/// </summary>
		internal IntPtr BaseAddress { get; }

		/// <summary>
		///     A pointer to the base address of a range of allocated pages. The page pointed to by the
		///     <see cref="BaseAddress" /> member is contained within this allocation range.
		/// </summary>
		internal IntPtr AllocationBase { get; }

		/// <summary>
		///     The memory protection option when the region was initially allocated. This member can be one of the
		///     following constants defined in wdm.h, or 0 if the caller does not have access.
		/// </summary>
		internal MemoryProtection AllocationProtect { get; }

		/// <summary>
		///     The size of the region in bytes beginning at the base address in which all pages have identical attributes.
		/// </summary>
		internal IntPtr RegionSize { get; }

		/// <summary>
		///     The state of the pages in the region.
		/// </summary>
		internal MemoryState State { get; }

		/// <summary>
		///     The access protection of the pages in the region. This member is one of the values listed for the
		///     <see cref="AllocationProtect" /> member.
		/// </summary>
		internal MemoryProtection Protect { get; }

		/// <summary>
		///     The type of pages in the region.
		/// </summary>
		internal MemoryType Type { get; }
	}
}
using System;
using System.Runtime.InteropServices;
using NeoCore.CoreClr.Support;
using NeoCore.Import;
using NeoCore.Import.Attributes;
using NeoCore.Interop;
using NeoCore.Interop.Attributes;
using NeoCore.Memory;
using NeoCore.Memory.Pointers;

// ReSharper disable InconsistentNaming

namespace NeoCore.CoreClr.VM
{
	[ImportNamespace]
	[NativeStructure]
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct MethodDesc : IClrSource
	{
		[ImportMapField]
		private static readonly ImportMap Imports = new ImportMap();

		#region Fields

		internal ParameterInfo Flags3AndTokenRemainder { get; }

		internal byte ChunkIndex { get; }

		internal CodeInfo Code { get; }

		internal ushort SlotNumber { get; }

		internal MethodProperties Properties { get; }

		/// <summary>
		///     Valid only if the function is non-virtual,
		///     non-abstract, non-generic (size of this <see cref="MethodDesc"/> <c>== 16</c>)
		/// </summary>
		internal void* Function { get; }

		#endregion

		#region Accessors

		#region Flags

		internal MethodClassification Classification =>
			(MethodClassification) ((ushort) Properties & (ushort) MethodProperties.Classification);

		#endregion

		#endregion

		#region Import

		[ImportCall(ImportCallOptions.Map)]
		internal void Reset()
		{
			fixed (MethodDesc* value = &this) {
				Functions.Native.CallVoid((void*) Imports[nameof(Reset)], value);
			}
		}

		[ImportCall(ImportCallOptions.Map)]
		internal bool IsPointingToNativeCode()
		{
			fixed (MethodDesc* value = &this) {
				return Functions.Native.Call<bool>((void*) Imports[nameof(IsPointingToNativeCode)], value);
			}
		}


		internal void* NativeCode {
			[ImportProperty]
			get {
				fixed (MethodDesc* value = &this) {
					return Functions.Native.CallReturnPointer((void*) Imports[nameof(NativeCode)], value);
				}
			}
		}

		[ImportCall(ImportCallOptions.Map)]
		internal bool SetNativeCodeInterlocked(long p)
		{
			fixed (MethodDesc* value = &this) {
				return Functions.Native.Call<bool>((void*) Imports[nameof(SetNativeCodeInterlocked)],
				                                   value, (void*) p);
			}
		}

		internal int Token {
			[ImportCall("GetMemberDef", ImportCallOptions.Map)]
			get {
				fixed (MethodDesc* value = &this) {
					return Functions.Native.Call<int>((void*) Imports[nameof(Token)], value);
				}
			}
		}


		[Obsolete]
		[ImportCall(ImportCallOptions.Map)]
		internal void* GetILHeader(int fAllowOverrides)
		{
			fixed (MethodDesc* value = &this) {
				return Functions.Native.CallReturnPointer((void*) Imports[nameof(GetILHeader)], value, fAllowOverrides);
			}
		}


		internal long RVA {
			[ImportProperty]
			get {
				fixed (MethodDesc* value = &this) {
					return Functions.Native.Call<long>((void*) Imports[nameof(RVA)], value);
				}
			}
		}

		private static int Alignment {
			get {
				//int alignmentShift = 3;
				int alignmentShift = Mem.Is64Bit ? 3 : 2;
				int alignment      = 1 << alignmentShift;
				int alignmentMask  = alignment - 1;

				return alignment;
			}
		}

		internal Pointer<MethodDescChunk> MethodDescChunk {
			get {
				// PTR_MethodDescChunk(dac_cast<TADDR>(this) -(sizeof(MethodDescChunk) + (GetMethodDescIndex() * MethodDesc::ALIGNMENT)));

				var thisptr = Unsafe.AddressOf(ref this).Cast();
				return (thisptr - (sizeof(MethodDescChunk) + (ChunkIndex * Alignment)));
			}
		}

		internal Pointer<MethodTable> MethodTable => MethodDescChunk.Reference.MethodTable;

		#endregion
	}
}
using System.Runtime.InteropServices;
using Memkit;
using Memkit.Pointers;
using Memkit.Utilities;
using NeoCore.CoreClr.VM.Jit;
using NeoCore.Import;
using NeoCore.Import.Attributes;
using NeoCore.Model;
using NeoCore.Win32.Attributes;

// ReSharper disable InconsistentNaming

namespace NeoCore.CoreClr.VM
{
	[ImportNamespace]
	[NativeStructure]
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct MethodDesc : IClrStructure
	{
		[ImportMapField]
		private static readonly ImportMap Imports = new ImportMap();

		#region Fields

		internal ParamFlags Flags3AndTokenRemainder { get; }

		internal byte ChunkIndex { get; }

		internal CodeFlags Code { get; }

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
		internal bool IsPointingToNativeCode()
		{
			fixed (MethodDesc* value = &this) {
				return Imports.Call<bool, ulong>(nameof(IsPointingToNativeCode), (ulong) value);
			}
		}


		internal void* NativeCode {
			[ImportAccessor]
			get {
				fixed (MethodDesc* value = &this) {
					return Imports.CallReturnPointer(nameof(NativeCode), (ulong)value);
				}
			}
		}

		[ImportCall(ImportCallOptions.Map)]
		internal bool SetNativeCodeInterlocked(long p)
		{
			fixed (MethodDesc* value = &this) {
				return Imports.Call<bool, ulong, ulong>(nameof(SetNativeCodeInterlocked), 
				                                        (ulong) value, (ulong)p);
			}
		}

		internal int Token {
			[ImportCall("GetMemberDef", ImportCallOptions.Map)]
			get {
				fixed (MethodDesc* value = &this) {
					return Imports.Call<int, ulong>(nameof(Token), (ulong) value);
				}
			}
		}

		
		[ImportCall(ImportCallOptions.Map)]
		internal CorMethod* GetILHeader(int fAllowOverrides)
		{
			fixed (MethodDesc* value = &this) {
				return (CorMethod*) Imports.CallReturnPointer(nameof(GetILHeader), (ulong) value, fAllowOverrides);
			}
		}


		internal long RVA {
			[ImportAccessor]
			get {
				fixed (MethodDesc* value = &this) {
					return Imports.Call<long, ulong>(nameof(RVA), (ulong) value);
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

				var thisptr = Mem.AddressOf(ref this).Cast();
				return (thisptr - (sizeof(MethodDescChunk) + (ChunkIndex * Alignment)));
			}
		}

		internal Pointer<MethodTable> MethodTable => MethodDescChunk.Reference.MethodTable;

		#endregion

		public ClrStructureType Type => ClrStructureType.Metadata;
		
		public string NativeName => nameof(MethodDesc);
	}
}
using System;
using NeoCore.CoreClr.Meta.Base;
using NeoCore.CoreClr.VM;
using NeoCore.CoreClr.VM.EE;
using NeoCore.Memory;
using NeoCore.Memory.Pointers;

// ReSharper disable InconsistentNaming

namespace NeoCore.CoreClr.Meta
{
	/// <summary>
	///     <list type="bullet">
	///         <item><description>CLR structure: <see cref="EEClassLayoutInfo"/></description></item>
	///         <item><description>Reflection structure: N/A</description></item>
	///     </list>
	/// </summary>
	public unsafe class MetaLayout : AnonymousClrStructure<EEClassLayoutInfo>
	{
		#region Constructors

		public MetaLayout(Pointer<EEClassLayoutInfo> ptr) : base(ptr) { }

		#endregion

		protected override Type[] AdditionalSources => Array.Empty<Type>();
		
		#region Accessors

		public int NativeSize => Value.Reference.NativeSize;

		public int ManagedSize => Value.Reference.ManagedSize;

		public LayoutFlags Flags => Value.Reference.Flags;

		public int PackingSize => Value.Reference.PackingSize;

		public int NumCTMFields => Value.Reference.NumCTMFields;

		public Pointer<byte> FieldMarshalers => Value.Reference.FieldMarshalers;

		#endregion
	}
}
using NeoCore.Import;
using NeoCore.Import.Providers;
using NeoCore.Utilities;
using NeoCore.Utilities.Diagnostics;
using NeoCore.Win32;

namespace NeoCore.Model
{
	/// <summary>
	///     <list type="bullet">
	///         <listheader>Inheriting types:</listheader>
	///         <item>
	///             <description>
	///                 <see cref="Global" />
	///             </description>
	///         </item>
	///         <item>
	///             <description>
	///                 <see cref="Functions.Native" />
	///             </description>
	///         </item>
	///         <item>
	///             <description>
	///                 <see cref="SymbolManager" />
	///             </description>
	///         </item>
	/// <item>
	///             <description>
	///                 <see cref="ImportManager" />
	///             </description>
	///         </item>
	///     </list>
	/// </summary>
	public abstract class Releasable : Closable
	{
		public Releasable() { }

		public bool IsSetup { get; protected set; }

		public virtual void Setup()
		{
			if (!IsSetup) {
				IsSetup = true;
			}
		}

		public override void Close()
		{
			if (IsSetup) {
				IsSetup = false;
				base.Close();
			}
		}
	}
}
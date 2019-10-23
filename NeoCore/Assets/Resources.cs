using System;
using System.Runtime.CompilerServices;
using NeoCore.CoreClr;
using NeoCore.CoreClr.Metadata;
using NeoCore.CoreClr.Metadata.EE;
using NeoCore.CoreClr.Support;
using NeoCore.Import;
using NeoCore.Model;
using NeoCore.Utilities.Diagnostics;

[assembly: InternalsVisibleTo("Test")]
namespace NeoCore.Assets
{
	internal static class Resources
	{
		/**
		 * todo list
		 *
		 * - Compare with RazorSharp
		 * - Add/improve missing features from RazorSharp
		 */
		
		static Resources()
		{
			Guard.AssertCompatibility();
		}

		private static readonly Type[] CoreClrTypes =
		{
			typeof(TypeHandle),
			typeof(MethodTable),
			typeof(MethodDesc),
			typeof(FieldDesc),
			typeof(EEClass),
		};
		
		private static readonly Closable[] CoreObjects =
		{
			Global.Value, 
			SymbolManager.Value, 
			ImportManager.Value,
		};
		
		internal static RuntimeAsset Clr { get; private set; } = new ClrRuntimeAsset(ClrFrameworks.Core);

		internal static void Close()
		{
			Clr = null;

			foreach (var value in CoreObjects) {
				value?.Close();
			}
		}
	}
}
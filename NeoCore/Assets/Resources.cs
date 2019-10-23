using System;
using System.Runtime.CompilerServices;
using NeoCore.CoreClr;
using NeoCore.CoreClr.Metadata;
using NeoCore.Import;
using NeoCore.Model;
using NeoCore.Utilities.Diagnostics;

[assembly: InternalsVisibleTo("Test")]
namespace NeoCore.Assets
{
	internal static class Resources
	{
		static Resources()
		{
			Guard.AssertCompatibility();
		}

		private static readonly Type[] CoreClrTypes =
		{
			typeof(TypeHandle),
			typeof(MethodTable),
			typeof(MethodDesc)
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
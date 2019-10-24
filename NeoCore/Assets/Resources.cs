using System;
using System.Runtime.CompilerServices;
using NeoCore.CoreClr;
using NeoCore.CoreClr.Metadata;
using NeoCore.CoreClr.Metadata.EE;
using NeoCore.CoreClr.Support;
using NeoCore.Import;
using NeoCore.Interop;
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
		 * - Clean up
		 */
		
		static Resources()
		{
			Guard.AssertCompatibility();
		}

		private static readonly Type[] CoreClrTypes =
		{
			typeof(TypeHandle),
			typeof(MethodDesc),
			typeof(FieldDesc),
			
//			typeof(MethodTable),
//			typeof(EEClass),
//			typeof(MethodDescChunk),
//			typeof(FunctionFactory),
		};
		
		private static readonly Closable[] CoreObjects =
		{
			SymbolManager.Value,
			ImportManager.Value,
			
			Global.Value, 
		};
		
		internal static RuntimeAsset Clr { get; private set; } = new ClrRuntimeAsset(ClrFrameworks.Core);

		internal static void SetupAll()
		{
			ImportManager.Value.LoadAll(CoreClrTypes, Clr.Imports);
			
			var appDomain = AppDomain.CurrentDomain;
			appDomain.ProcessExit += (sender, eventArgs) => { Close(); };
		}
		
		internal static void Close()
		{
			Clr = null;

			foreach (var value in CoreObjects) {
				value?.Close();
			}
			
			
		}
	}
}
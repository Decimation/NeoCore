using System;
using System.Runtime.CompilerServices;
using NeoCore.CoreClr;
using NeoCore.CoreClr.Support;
using NeoCore.CoreClr.VM;
using NeoCore.CoreClr.VM.EE;
using NeoCore.Import;
using NeoCore.Interop;
using NeoCore.Model;
using NeoCore.Utilities.Diagnostics;

[assembly: InternalsVisibleTo("Test")]
namespace NeoCore.Assets
{
	/// <summary>
	/// Contains core resources for NeoCore.
	/// </summary>
	internal static class Resources
	{
		/**
		 * todo list
		 *
		 * - Compare with RazorSharp
		 * - Add/improve missing features from RazorSharp
		 * - Clean up
		 */

		private static bool IsSetup { get; set; } = false;
		
		static Resources()
		{
			Guard.AssertCompatibility();
			SetupAll();
		}

		private static readonly Type[] CoreClrTypes =
		{
			typeof(TypeHandle),
			typeof(MethodDesc),
			typeof(FieldDesc),
			typeof(GCHeap),
			typeof(MethodTable),
			typeof(EEClass),
			typeof(EEClassLayoutInfo),
			typeof(MethodDescChunk),
			typeof(FunctionFactory),
			typeof(Globals),
		};
		
		private static readonly Closable[] CoreObjects =
		{
			SymbolManager.Value,
			ImportManager.Value,
			CoreLog.Value, 
		};
		
		internal static RuntimeAsset Clr { get; private set; } = new ClrRuntimeAsset(ClrFrameworks.Core);

		private static void SetupAll()
		{
			if (IsSetup) {
				return;
			}
			
//			ImportManager.Value.LoadAll(CoreClrTypes, Clr.Imports);
			
			var appDomain = AppDomain.CurrentDomain;
			appDomain.ProcessExit += (sender, eventArgs) => { Close(); };

			IsSetup = true;
		}

		private static void Close()
		{
			Clr = null;

			foreach (var value in CoreObjects) {
				value?.Close();
			}

			IsSetup = false;
		}
	}
}
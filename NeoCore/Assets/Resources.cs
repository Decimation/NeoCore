using System;
using System.Runtime.CompilerServices;
using NeoCore.CoreClr;
using NeoCore.CoreClr.Components;
using NeoCore.CoreClr.Components.Support;
using NeoCore.CoreClr.Components.VM;
using NeoCore.CoreClr.Components.VM.EE;
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

		/**
		 * - https://docs.microsoft.com/en-us/dotnet/standard/design-guidelines/names-of-namespaces?redirectedfrom=MSDN
		 * - <Company>.(<Product>|<Technology>)[.<Feature>][.<Subnamespace>]
		 */

		private static bool IsSetup { get; set; }

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
			typeof(Globals),
		};

		private static readonly Closable[] CoreObjects =
		{
			SymbolManager.Value,
			ImportManager.Value,
			CoreLogger.Value,
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
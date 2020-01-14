using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using NeoCore.CoreClr;
using NeoCore.CoreClr.VM;
using NeoCore.CoreClr.VM.EE;
using NeoCore.Import;
using NeoCore.Import.Providers;
using NeoCore.Model;
using NeoCore.Utilities;
using NeoCore.Utilities.Diagnostics;

[assembly: InternalsVisibleTo("Test")]

namespace NeoCore.Support
{
	/// <summary>
	/// Contains core resources for NeoCore.
	/// </summary>
	internal static class Resources
	{
		/// <summary>
		/// Name of this assembly.
		/// </summary>
		public const string NAME = "NeoCore";

		// todo: portability
		private const string IDX = @"C:\Users\Deci\RiderProjects\NeoCore\NeoCore\clr_image.json";

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
		};

		private static readonly Closable[] CoreObjects =
		{
			SymbolManager.Value,
			ImportManager.Value,
			Global.Value,
		};

		static Resources()
		{
			Guard.AssertCompatibility();
			SetupAll();
		}

		// - https://docs.microsoft.com/en-us/dotnet/standard/design-guidelines/names-of-namespaces?redirectedfrom=MSDN
		// - <Company>.(<Product>|<Technology>)[.<Feature>][.<Subnamespace>]

		private static bool IsSetup { get; set; }

		internal static ClrFramework Framework => ClrFrameworks.Core;

		internal static LibraryProcess CurrentProcess => Process.GetCurrentProcess();

		internal static ClrRuntimeAsset Clr { get; private set; } = new ClrRuntimeAsset(Framework, IDX);

		internal static void FullSetup()
		{
			foreach (var type in CoreClrTypes) {
				RuntimeHelpers.RunClassConstructor(type.TypeHandle);
			}

			ImportManager.Value.LoadAll(CoreClrTypes, Clr.Imports);
		}

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
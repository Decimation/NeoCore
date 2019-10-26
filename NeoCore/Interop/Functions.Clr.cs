using System;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace NeoCore.Interop
{
	public static unsafe partial class Functions
	{
		internal static class Clr
		{
			static Clr()
			{
				const string FN_NAME = "GetTypeFromHandleUnsafe";
				ReadTypeFromHandle = Reflection.FindFunction<GetTypeFromHandleSig, Type>(FN_NAME);
				
				const string FN2_NAME = "IsPinnable";
				IsPinnable = Reflection.FindFunction<IsPinnableSig>(typeof(Marshal),FN2_NAME);
			}
			
			internal delegate bool IsPinnableSig([CanBeNull] object handle);
			
			internal delegate Type GetTypeFromHandleSig(IntPtr handle);

			internal static GetTypeFromHandleSig ReadTypeFromHandle { get; }
			
			internal static IsPinnableSig IsPinnable { get; }
		}
	}
}
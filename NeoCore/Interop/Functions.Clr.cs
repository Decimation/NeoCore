using System;

namespace NeoCore.Interop
{
	public static unsafe partial class Functions
	{
		internal static class Clr
		{
			static Clr()
			{
				const string FN_NAME = "GetTypeFromHandleUnsafe";
				ReadTypeFromHandle = Reflection.FindFunction<GetTypeFromHandle, Type>(FN_NAME);
			}

			internal delegate Type GetTypeFromHandle(IntPtr handle);

			internal static GetTypeFromHandle ReadTypeFromHandle { get; }
		}
	}
}
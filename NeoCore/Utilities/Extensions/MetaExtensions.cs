using System;
using System.Reflection;
using NeoCore.CoreClr.Meta;

namespace NeoCore.Utilities.Extensions
{
	public static class MetaExtensions
	{
		public static MetaType AsMetaType(this Type t) => t;

		public static MetaField AsMetaField(this FieldInfo t) => t;

		public static MetaMethod AsMetaMethod(this MethodInfo t) => t;
	}
}
using System;
using System.Linq;
using System.Reflection;
using System.Text;
using SimpleCore.Formatting;

// ReSharper disable ReturnTypeCanBeEnumerable.Global

namespace NeoCore.Utilities
{
	/// <summary>
	/// Provides Reflection utilities.
	/// </summary>
	public static class EasyReflection
	{
		// https://docs.microsoft.com/en-us/dotnet/framework/reflection-and-codedom/specifying-fully-qualified-type-names

		public static Type? GetTypeSimple(this Assembly asm, string name)
		{
			return asm.GetType(asm.GetName().Name + Format.PERIOD + name);
		}

		public static MemberInfo[] MemberOf(string type, string name)
		{
			return Assembly.GetCallingAssembly()
			               .GetTypeSimple(type)
			               .GetAnyMember(name);
		}

		public static FieldInfo FieldOf(string type, string name)
		{
			return Assembly.GetCallingAssembly()
			               .GetTypeSimple(type)
			               .GetAnyField(name);
		}

		public static MethodInfo MethodOf(string type, string name)
		{
			return Assembly.GetCallingAssembly()
			               .GetTypeSimple(type)
			               .GetAnyMethod(name);
		}

		#region Flags

		/// <summary>
		///     <see cref="ALL_INSTANCE_FLAGS" /> and <see cref="BindingFlags.Static" />
		/// </summary>
		public const BindingFlags ALL_FLAGS = ALL_INSTANCE_FLAGS | BindingFlags.Static;

		/// <summary>
		///     <see cref="BindingFlags.Public" />, <see cref="BindingFlags.Instance" />,
		///     and <see cref="BindingFlags.NonPublic" />
		/// </summary>
		private const BindingFlags ALL_INSTANCE_FLAGS =
			BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic;

		#endregion

		#region Member

		internal static MemberInfo[] GetAllMembers(this Type t) => t.GetMembers(ALL_FLAGS);

		internal static MemberInfo[] GetAnyMember(this Type t, string name) =>
			t.GetMember(name, ALL_FLAGS);

		#endregion

		#region Field

		internal static FieldInfo GetAnyField(this Type t, string name) =>
			t.GetField(name, ALL_FLAGS);

		internal static FieldInfo[] GetAllFields(this Type t) => t.GetFields(ALL_FLAGS);

		#endregion

		#region Methods

		internal static MethodInfo[] GetAllMethods(this Type t) => t.GetMethods(ALL_FLAGS);

		internal static MethodInfo GetAnyMethod(this Type t, string name) =>
			t.GetMethod(name, ALL_FLAGS);

		#endregion

		#region Attributes

		internal static AnnotatedMember<TAttr> GetFirstAnnotated<TAttr>(this Type t) where TAttr : Attribute
		{
			AnnotatedMember<TAttr>[] rg = t.GetAnnotated<TAttr>();

			return rg.Length == default ? new AnnotatedMember<TAttr>() : rg[0];
		}

		internal static AnnotatedMember<TAttr>[] GetAnnotated<TAttr>(this Type t) where TAttr : Attribute
		{
			return (from member in t.GetAllMembers()
			        where Attribute.IsDefined(member, typeof(TAttr))
			        select new AnnotatedMember<TAttr>(member, member.GetCustomAttribute<TAttr>())).ToArray();
		}

		#endregion
	}
}
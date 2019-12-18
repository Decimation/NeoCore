using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

// ReSharper disable ReturnTypeCanBeEnumerable.Global

namespace NeoCore.Utilities.Extensions
{
	/// <summary>
	/// Provides utilities for accessing members of a type.
	/// </summary>
	internal static class ReflectionExtensions
	{
		#region Member

		internal static MemberInfo[] GetAllMembers(this Type t) => t.GetMembers(EasyReflection.ALL_FLAGS);

		internal static MemberInfo[] GetAnyMember(this Type t, string name) =>
			t.GetMember(name, EasyReflection.ALL_FLAGS);

		#endregion

		#region Field

		internal static FieldInfo GetAnyField(this Type t, string name) =>
			t.GetField(name, EasyReflection.ALL_FLAGS);

		internal static FieldInfo[] GetAllFields(this Type t) => t.GetFields(EasyReflection.ALL_FLAGS);

		#endregion

		#region Methods

		internal static MethodInfo[] GetAllMethods(this Type t) => t.GetMethods(EasyReflection.ALL_FLAGS);

		internal static MethodInfo GetAnyMethod(this Type t, string name) =>
			t.GetMethod(name, EasyReflection.ALL_FLAGS);

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

		public static bool ImplementsGenericInterface(this Type type, Type interfaceName)
		{
			bool IsMatch(Type t)
			{
				return t.IsGenericType && t.GetGenericTypeDefinition() == interfaceName;
			}

			return type.GetInterfaces().Any(IsMatch);
		}

		public static bool ImplementsInterface(this Type type, string interfaceName) =>
			type.GetInterface(interfaceName) != null;
	}
}
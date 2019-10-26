using System;
using System.Collections.Generic;
using System.Reflection;
// ReSharper disable ReturnTypeCanBeEnumerable.Global

namespace NeoCore.Utilities.Extensions
{
	/// <summary>
	/// Provides utilities for accessing members of a type.
	/// </summary>
	internal static class ReflectionExtensions
	{
		#region Flags

		/// <summary>
		///     <see cref="ALL_INSTANCE_FLAGS" /> and <see cref="BindingFlags.Static" />
		/// </summary>
		private const BindingFlags ALL_FLAGS = ALL_INSTANCE_FLAGS | BindingFlags.Static;

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
			var components = new List<AnnotatedMember<TAttr>>();

			foreach (var member in t.GetAllMembers()) {
				if (Attribute.IsDefined(member, typeof(TAttr))) {
					components.Add(new AnnotatedMember<TAttr>(member, member.GetCustomAttribute<TAttr>()));
				}
			}

			return components.ToArray();
		}

		#endregion
	}
}
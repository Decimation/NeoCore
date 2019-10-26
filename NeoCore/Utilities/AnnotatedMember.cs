using System;
using System.Reflection;

namespace NeoCore.Utilities
{
	/// <summary>
	/// Contains a <see cref="Attribute"/> and its accompanying <see cref="MemberInfo"/>
	/// </summary>
	/// <typeparam name="TAttr"><see cref="Attribute"/> type</typeparam>
	public readonly struct AnnotatedMember<TAttr> where TAttr : Attribute
	{
		/// <summary>
		/// Decorated member.
		/// </summary>
		public MemberInfo Member { get; }

		/// <summary>
		/// Attribute decorating <see cref="Member"/>
		/// </summary>
		public TAttr Attribute { get; }
		
		public bool IsNull { get; }

		public AnnotatedMember(MemberInfo memberInfo, TAttr attribute)
		{
			(Member, Attribute) = (memberInfo, attribute);
			IsNull = Attribute == null && Member == null;
		}
	}
}
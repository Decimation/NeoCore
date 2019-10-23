using System;

namespace NeoCore.CoreClr.Metadata
{
	[Flags]
	public enum FieldProperties
	{
		// <summary>
		// <c>DWORD</c> #1
		//     <para>unsigned m_mb : 24;</para>
		//     <para>unsigned m_isStatic : 1;</para>
		//     <para>unsigned m_isThreadLocal : 1;</para>
		//     <para>unsigned m_isRVA : 1;</para>
		//     <para>unsigned m_prot : 3;</para>
		//     <para>unsigned m_requiresFullMbValue : 1;</para>
		// </summary>
		
		None = 0,
		
		Static = 1 << 24,
		
		ThreadLocal = 1 << 25,
		
		RVA = 1 << 26,
		
		RequiresFullMBValue = 1 << 31
	}
}
#region

using System;
using JetBrains.Annotations;

#endregion

namespace NeoCore.CoreClr.Framework
{
	/// <summary>
	/// Describes a .NET framework with its type and version.
	/// </summary>
	public struct FrameworkIdentifier
	{
		/// <summary>
		/// Framework type.
		/// </summary>
		public FrameworkTypes Type { get; }

		/// <summary>
		/// The version of the framework, if specified.
		/// </summary>
		public Version Version { get; }

		internal FrameworkIdentifier(FrameworkTypes fwk, [CanBeNull] Version vsn)
		{
			(Type, Version) = (fwk, vsn);
		}


		public override string ToString()
		{
			string fwkStr = Type switch
			{
				FrameworkTypes.Standard => ".NET Standard",
				FrameworkTypes.Core => ".NET Core",
				FrameworkTypes.Framework => ".NET Framework",
				_ => throw new ArgumentOutOfRangeException()
			};

			bool hasVersion = Version != null;
			
			return hasVersion ? String.Format("{0} {1}", fwkStr, Version) : fwkStr;
		}
	}
}
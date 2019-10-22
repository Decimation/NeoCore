using System;
using JetBrains.Annotations;

namespace NeoCore
{
	public struct FrameworkIdentifier
	{
		public FrameworkTypes Type { get; }

		public Version Version { get; }

		internal FrameworkIdentifier(FrameworkTypes framework, [CanBeNull] Version version)
		{
			(Type, Version) = (framework, version);
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

			string verStr = Version == null ? String.Empty : Version.ToString();

			return String.Format("{0} {1}", fwkStr, verStr);
		}
	}
}
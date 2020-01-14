using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.Json.Serialization;
using NeoCore.Utilities;
using SimpleCore;
using SimpleCore.Formatting;

namespace NeoCore.Import.Providers
{
	public struct ImageRecord
	{
		public ImageRecordInfo Info { get; set; }

		public Dictionary<string, ImageRecordEntry[]> Records { get; set; }
	}

	public readonly struct ImageRecordEntry
	{
		internal ImageRecordEntry(string name, EntryType t, object v)
		{
			Name  = name;
			Type  = t;
			Value = v;
		}

		public string Name { get; }

		public EntryType Type { get; }

		/// <summary>
		/// <see cref="byte"/> array if <see cref="Type"/> is <see cref="EntryType.Signature"/>,
		/// <see cref="long"/> if <see cref="Type"/> is <see cref="EntryType.Offset"/>
		/// </summary>
		public object Value { get; }

		public bool IsNull => Name == null && Type == EntryType.Null && Value == null;

		public bool Equals(ImageRecordEntry other)
		{
			return Name == other.Name && Type == other.Type && Value.Equals(other.Value);
		}

		public override bool Equals(object? obj)
		{
			return obj is ImageRecordEntry other && Equals(other);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(Name, (int) Type, Value);
		}

		public static bool operator ==(ImageRecordEntry left, ImageRecordEntry right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(ImageRecordEntry left, ImageRecordEntry right)
		{
			return !left.Equals(right);
		}

		public override string ToString()
		{
			string       valStr  = null;
			const string FMT_HEX = "X";

			switch (Type) {
				case EntryType.Signature:
					var rg = Value as byte[];
					valStr = rg.FormatJoin(FMT_HEX);
					break;
				case EntryType.Offset:
					long l = (long) Value;
					valStr = l.ToString(FMT_HEX);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			return String.Format("{0}: {1} ({2})", Name, valStr, Type);
		}
	}

	public readonly struct ImageRecordInfo
	{
		internal ImageRecordInfo(string module, Version v)
		{
			Module  = module;
			Version = v;
		}

		public string  Module  { get; }
		public Version Version { get; }

		internal bool Check(ProcessModule module)
		{
			bool nameCheck = module.ModuleName == Module;

			var info = module.FileVersionInfo;

			var moduleVersion = new Version(info.FileMajorPart, info.FileMinorPart,
			                                info.FileBuildPart, info.FilePrivatePart);


			bool versionCheck = moduleVersion == Version;

			return nameCheck && versionCheck;
		}

		public override string ToString()
		{
			var sb = new StringBuilder();
			sb.AppendFormat("Module: {0}\n", Module);
			sb.AppendFormat("Version: {0}\n", Version);
			return sb.ToString();
		}
	}

	public enum EntryType
	{
		Null,
		Signature,
		Offset
	}
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Memkit;
using Memkit.Pointers;
using Memkit.Utilities;
using NeoCore.Support;
using NeoCore.Utilities;
using NeoCore.Utilities.Diagnostics;

namespace NeoCore.Import.Providers
{
	public sealed class ImageRecordImport : ImportProvider
	{
		public const string FILENAME = "clr_image.json";

		private readonly SigScanner m_scanner;

		public ImageRecordImport(string file, ProcessModule module) : base(module)
		{
			string txt = File.ReadAllText(file);


			var options = new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true,
				WriteIndented               = true,
			};

			options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
			options.Converters.Add(new ImageRecordEntryConverter());
			options.Converters.Add(new ImageRecordInfoConverter());


			var fullIndex = JsonSerializer.Deserialize<ImageRecord>(txt, options);
			Records = Compact(fullIndex.Records);
			Info    = fullIndex.Info;


			Guard.Assert(Info.Check(module));


			m_scanner = new SigScanner(module);

			Global.Value.WriteInfo(null, "Initialized with {Count} records", Records.Length);
		}

		public ImageRecordInfo Info { get; }

		public ImageRecordEntry[] Records { get; }

		// https://github.com/alliedmodders/sourcemod/blob/master/gamedata/sm-tf2.games.txt

		public ImageRecordEntry this[string id] {
			get { return Records.FirstOrDefault(r => r.Name == id); }
		}


		private static ImageRecordEntry[] Compact(Dictionary<string, ImageRecordEntry[]> fullIndex)
		{
			int compactLength = fullIndex.Values.Sum(v => v.Length);
			var compactIndex  = new List<ImageRecordEntry>(compactLength);

			// Special case
			const string GLOBAL_KEY = "#Global";
			compactIndex.AddRange(fullIndex[GLOBAL_KEY]);

			foreach (string enclosingName in fullIndex.Keys) {
				if (enclosingName == GLOBAL_KEY) {
					continue;
				}

				foreach (var oldEntry in fullIndex[enclosingName]) {
					string   memberName = oldEntry.Name;
					string[] scopes     = {enclosingName, memberName};
					string   fullName   = ImportManager.ScopeJoin(scopes);

					var compactIndexEntry =
						new ImageRecordEntry(fullName, oldEntry.Type, oldEntry.Value);
					compactIndex.Add(compactIndexEntry);
				}
			}


			return compactIndex.ToArray();
		}

		public override Pointer<byte> GetAddress(string id)
		{
			var entry = this[id];


			if (entry.IsNull) {
				string msg = String.Format("No entry for {0}", id);
				throw new InvalidOperationException(msg);
			}

			switch (entry.Type) {
				case EntryType.Signature:
					return m_scanner.FindPattern((byte[]) entry.Value);
				case EntryType.Offset:
					var modPtr = (Pointer<byte>) Module.BaseAddress;
					return modPtr.Add((long) entry.Value);
				case EntryType.Null:
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			throw new InvalidOperationException();
		}

		public override Pointer<byte>[] GetAddresses(string[] ids)
		{
			throw new System.NotImplementedException();
		}

		public override string ToString()
		{
			return Info.ToString();
		}
	}
}
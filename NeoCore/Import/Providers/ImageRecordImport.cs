using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using NeoCore.Memory.Pointers;

namespace NeoCore.Import.Providers
{
	public sealed class ImageRecordImport : ImportProvider
	{
		public ImageRecordEntry[] Records { get; }

		// https://github.com/alliedmodders/sourcemod/blob/master/gamedata/sm-tf2.games.txt

		public ImageRecordImport(string file, Pointer<byte> baseAddr) : base(baseAddr)
		{
			string txt = File.ReadAllText(file);

			var options = new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true,
				WriteIndented               = true,
			};
			options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
			options.Converters.Add(new ImageRecordEntryConverter());


			var fullIndex = JsonSerializer.Deserialize<Dictionary<string, ImageRecordEntry[]>>(txt, options);
			Records = Compact(fullIndex);
		}


		private static ImageRecordEntry[] Compact(Dictionary<string, ImageRecordEntry[]> fullIndex)
		{
			var compactLength = fullIndex.Values.Sum(v => v.Length);
			var compactIndex  = new List<ImageRecordEntry>(compactLength);

			// Special case
			const string GLOBAL_KEY = "Global";
			compactIndex.AddRange(fullIndex[GLOBAL_KEY]);

			foreach (string enclosingName in fullIndex.Keys) {
				if (enclosingName == GLOBAL_KEY) {
					continue;
				}

				foreach (var oldEntry in fullIndex[enclosingName]) {
					string memberName = oldEntry.Name;
					string[] scopes     = {enclosingName, memberName};
					string fullName   = ImportManager.ScopeJoin(scopes);

					var compactIndexEntry = new ImageRecordEntry(fullName, oldEntry.Type, oldEntry.Value);
					compactIndex.Add(compactIndexEntry);
				}
			}


			return compactIndex.ToArray();
		}

		public override Pointer<byte> GetAddress(string id)
		{
			throw new System.NotImplementedException();
		}

		public override Pointer<byte>[] GetAddresses(string[] ids)
		{
			throw new System.NotImplementedException();
		}
	}
}
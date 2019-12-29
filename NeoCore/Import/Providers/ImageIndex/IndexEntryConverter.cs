using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using NeoCore.Utilities;

namespace NeoCore.Import.Providers.ImageIndex
{
	internal sealed class IndexEntryConverter : JsonConverter<IndexEntry>
	{
		public override IndexEntry Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			if (reader.TokenType != JsonTokenType.StartObject) {
				throw new JsonException();
			}

			string    name  = null;
			EntryType type  = default;
			object    value = null;

			while (reader.Read()) {
				if (reader.TokenType == JsonTokenType.EndObject) {
					return new IndexEntry(name, type, value);
				}

				// Get the key.
				if (reader.TokenType != JsonTokenType.PropertyName) {
					throw new JsonException();
				}
				
				if (reader.GetString() == nameof(IndexEntry.Name).ToLower()) {
					reader.Read();
					name = reader.GetString();
				}

				if (reader.GetString() == nameof(IndexEntry.Type).ToLower()) {
					reader.Read();
					type = Enum.Parse<EntryType>(reader.GetString(), true);
				}

				if (reader.GetString() == nameof(IndexEntry.Value).ToLower()) {
					reader.Read();
					string rawValue = reader.GetString();

					value = type switch
					{
						EntryType.Signature => (object) SigScanner.ParseByteArray(rawValue),
						EntryType.Offset => Int64.Parse(rawValue, NumberStyles.HexNumber),
						_ => throw new JsonException()
					};
				}
			}

			throw new JsonException();
		}

		public override void Write(Utf8JsonWriter writer, IndexEntry value, JsonSerializerOptions options)
		{
			throw new NotImplementedException();
		}
	}
}
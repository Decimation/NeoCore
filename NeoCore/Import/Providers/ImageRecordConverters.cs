using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Memkit;
using Memkit.Utilities;
using NeoCore.Memory;
using NeoCore.Utilities;

namespace NeoCore.Import.Providers
{
	internal sealed class ImageRecordEntryConverter : JsonConverter<ImageRecordEntry>
	{
		public override ImageRecordEntry Read(ref Utf8JsonReader    reader, Type typeToConvert,
		                                      JsonSerializerOptions options)
		{
			if (reader.TokenType != JsonTokenType.StartObject) {
				throw new JsonException();
			}

			string    name  = null;
			EntryType type  = default;
			object    value = null;
			string alias = null;

			while (reader.Read()) {
				if (reader.TokenType == JsonTokenType.EndObject) {
					return new ImageRecordEntry(name, type, value, alias);
				}

				// Get the key.
				if (reader.TokenType != JsonTokenType.PropertyName) {
					throw new JsonException();
				}

				if (reader.GetString() == nameof(ImageRecordEntry.Name).ToLower()) {
					reader.Read();
					name = reader.GetString();
				}

				if (reader.GetString() == nameof(ImageRecordEntry.Type).ToLower()) {
					reader.Read();
					type = Enum.Parse<EntryType>(reader.GetString(), true);
				}

				if (reader.GetString() == nameof(ImageRecordEntry.Value).ToLower()) {
					reader.Read();
					string rawValue = reader.GetString();

					value = type switch
					{
						EntryType.Signature => (object) SigScanner.ParseByteArray(rawValue),
						EntryType.Offset => Int64.Parse(rawValue, NumberStyles.HexNumber),
						_ => throw new JsonException()
					};
				}
				
				if (reader.GetString() == nameof(ImageRecordEntry.Alias).ToLower()) {
					reader.Read();
					alias = reader.GetString();
				}
			}

			throw new JsonException();
		}

		public override void Write(Utf8JsonWriter writer, ImageRecordEntry value, JsonSerializerOptions options)
		{
			throw new NotImplementedException();
		}
	}

	internal sealed class ImageRecordInfoConverter : JsonConverter<ImageRecordInfo>
	{
		public override ImageRecordInfo Read(ref Utf8JsonReader    reader, Type typeToConvert,
		                                     JsonSerializerOptions options)
		{
			if (reader.TokenType != JsonTokenType.StartObject) {
				throw new JsonException();
			}

			string  module = null;
			Version v      = null;

			while (reader.Read()) {
				if (reader.TokenType == JsonTokenType.EndObject) {
					return new ImageRecordInfo(module, v);
				}

				// Get the key.
				if (reader.TokenType != JsonTokenType.PropertyName) {
					throw new JsonException();
				}

				if (reader.GetString() == nameof(ImageRecordInfo.Module).ToLower()) {
					reader.Read();
					module = reader.GetString();
				}

				if (reader.GetString() == nameof(ImageRecordInfo.Version).ToLower()) {
					reader.Read();
					v = Version.Parse(reader.GetString());
				}
			}

			throw new JsonException();
		}

		public override void Write(Utf8JsonWriter writer, ImageRecordInfo value, JsonSerializerOptions options)
		{
			throw new NotImplementedException();
		}
	}
}
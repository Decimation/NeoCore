using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using NeoCore.Utilities;

namespace NeoCore.Import.Providers
{
	internal sealed class ImageRecordInfoConverter : JsonConverter<ImageRecordInfo>
	{
		public override ImageRecordInfo Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			if (reader.TokenType != JsonTokenType.StartObject) {
				throw new JsonException();
			}

			string module = null;
			Version v = null;

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
using System.Text.Json;
using System.Text.Json.Serialization;
using EHunter.EHentai.Api.Models;

namespace EHunter.EHentai.Api.Json
{
    internal class TagConverter : JsonConverter<Tag>
    {
        public override Tag Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.String)
                throw new JsonException("String expected for tag.");

            string value = reader.GetString()!;
            int index = value.IndexOf(':', StringComparison.Ordinal);
            if (index == -1)
            {
                return new(null, value);
            }
            else
            {
                return new(value[..index], value[(index + 1)..]);
            }
        }

        public override void Write(Utf8JsonWriter writer, Tag value, JsonSerializerOptions options)
        {
            if (value.Namespace == null)
                writer.WriteStringValue(value.Name);
            else
                writer.WriteStringValue($"{value.Namespace}:{value.Name}");
        }
    }
}

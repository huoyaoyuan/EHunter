using System;
using System.Collections.Immutable;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EHunter.EHentai.Api.Json
{
    public sealed record EHentaiApiResponse(string? Error,
        [property: JsonPropertyName("gmetadata")] ImmutableArray<GalleryMetadata> Galleries);

    public sealed record GalleryMetadata(
        int Gid,
        string Token,
        [property: JsonPropertyName("archiver_key")] string ArchiverKey,
        string Title,
        [property: JsonPropertyName("title_jpn")] string TitleJpn,
        [property: JsonConverter(typeof(GalleryCategoryConverter))] GalleryCategory Category,
        Uri Thumb,
        string Uploader,
        [property: JsonConverter(typeof(EHentaiTimeStampConverter))] DateTimeOffset Posted,
        [property: JsonPropertyName("filecount")] int FileCount,
        [property: JsonPropertyName("filesize")] long FileSize,
        bool Expunged,
        decimal Rating,
        int TorrentCount,
        ImmutableArray<string> Tags);

    internal sealed class EHentaiTimeStampConverter : JsonConverter<DateTimeOffset>
    {
        public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => DateTimeOffset.FromUnixTimeSeconds(long.Parse(reader.GetString()!, NumberFormatInfo.InvariantInfo));

        public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
            => writer.WriteStringValue(value.ToUnixTimeSeconds().ToString(NumberFormatInfo.InvariantInfo));
    }
}

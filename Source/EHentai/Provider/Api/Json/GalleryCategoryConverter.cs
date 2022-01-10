using System.Text.Json;
using System.Text.Json.Serialization;

namespace EHunter.EHentai.Api.Json
{
    internal sealed class GalleryCategoryConverter : JsonConverter<GalleryCategory>
    {
        public override GalleryCategory Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => reader.GetString() switch
            {
                "Doujinshi" => GalleryCategory.Doujinshi,
                "Manga" => GalleryCategory.Manga,
                "Artist CG" => GalleryCategory.ArtistCG,
                "Game CG" => GalleryCategory.GameCG,
                "Western" => GalleryCategory.Western,
                "Non-H" => GalleryCategory.NonH,
                "Image Set" => GalleryCategory.ImageSet,
                "Cosplay" => GalleryCategory.Cosplay,
                "Asian Porn" => GalleryCategory.AsianPorn,
                "Misc" => GalleryCategory.Misc,
                var other => throw new InvalidOperationException($"Unknown category value {other}")
            };

        public override void Write(Utf8JsonWriter writer, GalleryCategory value, JsonSerializerOptions options)
            => writer.WriteStringValue(value switch
            {
                GalleryCategory.Doujinshi => "Doujinshi",
                GalleryCategory.Manga => "Manga",
                GalleryCategory.ArtistCG => "Artist CG",
                GalleryCategory.GameCG => "Game CG",
                GalleryCategory.Western => "Western",
                GalleryCategory.NonH => "Non-H",
                GalleryCategory.ImageSet => "Image Set",
                GalleryCategory.Cosplay => "Cosplay",
                GalleryCategory.AsianPorn => "Asian Porn",
                GalleryCategory.Misc => "Misc",
                var other => throw new InvalidOperationException($"Unknown category value {other}")
            });
    }
}

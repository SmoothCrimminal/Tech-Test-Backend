using System.Text.Json.Serialization;

namespace TechTestBackend.Dtos
{
    public class SpotifySongsWrapperDto
    {
        [JsonPropertyName("tracks")]
        public Tracks? Tracks { get; set; }
    }

    public class Tracks
    {
        [JsonPropertyName("items")]
        public List<SpotifySongDto>? Songs { get; set; }
    }
}

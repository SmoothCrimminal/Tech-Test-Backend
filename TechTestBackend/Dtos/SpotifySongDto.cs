using System.Text.Json.Serialization;

namespace TechTestBackend.Dtos
{
    public class SpotifySongDto
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("id")]
        public string? Id { get; set; }
    }
}

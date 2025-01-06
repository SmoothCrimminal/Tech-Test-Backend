using System.Net.Http.Headers;
using TechTestBackend.Constants;
using TechTestBackend.Dtos;
using TechTestBackend.Interfaces;

namespace TechTestBackend.Services
{
    public class SpotifyService : ITracksService
    {
        private HttpClient _spotifyClient;

        public SpotifyService(IHttpClientFactory httpClientFactory)
        {
            _spotifyClient = httpClientFactory.CreateClient(ApiConstants.SpotifyApiClient);    
        }

        public async Task<SpotifySongDto?> GetTrackAsync(string id)
        {
            var track = await _spotifyClient.GetFromJsonAsync<SpotifySongDto>($"/v1/tracks/{id}/");
            return track;
        }

        public async Task<IEnumerable<SpotifySongDto>?> GetTracksAsync(string trackName)
        {
            var wrappedDto = await _spotifyClient.GetFromJsonAsync<SpotifySongsWrapperDto>($"/v1/search?q={trackName}&type=track");
            return wrappedDto?.Tracks?.Songs;
        }
    }
}

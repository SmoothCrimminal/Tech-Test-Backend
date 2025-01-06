using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;
using TechTestBackend.Configurations;
using TechTestBackend.Dtos;

namespace TechTestBackend.DelegatingHandlers
{
    public class SpotifyAuthenticationHandler : DelegatingHandler
    {
        private readonly SpotifyApiConfiguration _spotifyApiConfiguration;
        private static string? _accessToken = null;

        public SpotifyAuthenticationHandler(IOptions<SpotifyApiConfiguration> options)
        {
            _spotifyApiConfiguration = options.Value;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (_accessToken is not null)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);

                var response = await base.SendAsync(request, cancellationToken);

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || response.StatusCode == System.Net.HttpStatusCode.Forbidden) 
                {
                    var spotifyCredentials = await GetTokenAsync(cancellationToken);
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", spotifyCredentials?.AccessToken);
                    _accessToken = spotifyCredentials?.AccessToken;
                }

                return response;
            }
            else
            {
                var spotifyCredentials = await GetTokenAsync(cancellationToken);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", spotifyCredentials?.AccessToken);
                _accessToken = spotifyCredentials?.AccessToken;

                return await base.SendAsync(request, cancellationToken);
            }
        }

        private async Task<SpotifyClientDto?> GetTokenAsync(CancellationToken cancellationToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "https://accounts.spotify.com/api/token");
            request.Content = new FormUrlEncodedContent([new KeyValuePair<string, string>("grant_type", "client_credentials")]);
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_spotifyApiConfiguration.ClientId}:{_spotifyApiConfiguration.ClientSecret}")));
            var response = await base.SendAsync(request, cancellationToken);

            if (!response.IsSuccessStatusCode)
                throw new Exception("Cannot authorize Spotify Client");

            var spotifyCredentials = await response.Content.ReadFromJsonAsync<SpotifyClientDto>();
            return spotifyCredentials;
        }
    }
}

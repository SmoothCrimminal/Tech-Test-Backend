using Microsoft.EntityFrameworkCore;
using TechTestBackend.Dtos;
using TechTestBackend.Interfaces;

namespace TechTestBackend.Services
{
    public class SpotifyService
    {
        private readonly SongsStorageContext _songsStorageContext;
        private readonly IRemoteTracksService _remoteTracksService;

        public SpotifyService(SongsStorageContext songsStorageContext, IRemoteTracksService remoteTracksService)
        {
            _songsStorageContext = songsStorageContext;
            _remoteTracksService = remoteTracksService;
        }

        private bool HasCorrectId(string id) => id.Length == 22;

        public async Task<SpotifySongDto?> GetTrackAsync(string trackId) => await _remoteTracksService.GetTrackAsync(trackId);

        public async Task<IEnumerable<SpotifySongDto>?> GetTracksByNameAsync(string trackName) => await _remoteTracksService.GetTracksByNameAsync(trackName);

        public async Task AddSongAsync(string songId)
        {
            if (!HasCorrectId(songId))
                return;

            var song = _songsStorageContext.Songs.FirstOrDefault(song => song.Id == songId);
            if (song is not null)
                return;

            var track = await GetTrackAsync(songId);
            if (track is null)
                return;

            song = new SpotifySong
            {
                Id = track.Id,
                Name = track.Name
            };

            await _songsStorageContext.Songs.AddAsync(song);
            await _songsStorageContext.SaveChangesAsync();
        }

        public async Task RemoveSongAsync(string songId)
        {
            if (!HasCorrectId(songId))
                return;

            var songToRemove = await _songsStorageContext.Songs.FirstOrDefaultAsync(song => song.Id == songId);
            if (songToRemove is null) 
                return;

            _songsStorageContext.Songs.Remove(songToRemove);
            await _songsStorageContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<SpotifySong>> ListAsync()
        {
            var likedSongs = new List<SpotifySong>();
            var wasAnySongRemoved = false;

            var songs = _songsStorageContext.Songs.AsNoTracking();
            if (!songs.Any())
                return likedSongs;

            foreach (var song in songs)
            {
                if (song is null) 
                    continue;

                var track = await GetTrackAsync(song.Id);
                if (track is null)
                {
                    wasAnySongRemoved = true;

                    _songsStorageContext.Songs.Remove(song);
                }
                else
                {
                    if (likedSongs.Any(ls => ls.Name == track.Name))
                        continue;

                    likedSongs.Add(song);
                }
            }

            if (wasAnySongRemoved)
            {
                await _songsStorageContext.SaveChangesAsync();
            }

            return likedSongs;
        }
    }
}

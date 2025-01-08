﻿using Microsoft.EntityFrameworkCore;
using TechTestBackend.Dtos;
using TechTestBackend.Interfaces;

namespace TechTestBackend.Services
{
    public class SpotifyService
    {
        private readonly SongsStorageContext _songsStorageContext;
        private readonly IRemoteTracksService _remoteTracksService;
        private readonly ILogger<SpotifyService> _logger;

        public SpotifyService(SongsStorageContext songsStorageContext, IRemoteTracksService remoteTracksService, ILogger<SpotifyService> logger)
        {
            _songsStorageContext = songsStorageContext;
            _remoteTracksService = remoteTracksService;
            _logger = logger;
        }

        private bool HasCorrectId(string id) => id.Length == 22;

        public async Task<SpotifySongDto?> GetTrackAsync(string trackId) => await _remoteTracksService.GetTrackAsync(trackId);

        public async Task<IEnumerable<SpotifySongDto>?> GetTracksByNameAsync(string trackName) => await _remoteTracksService.GetTracksByNameAsync(trackName);

        public async Task<Result> AddSongAsync(string songId)
        {
            if (!HasCorrectId(songId))
                return new Result().WithMessage($"The song with id: {songId} has invalid length").WithStatusCode(StatusCode.BadRequest);

            var song = _songsStorageContext.Songs.FirstOrDefault(song => song.Id == songId);
            if (song is not null)
                return new Result().WithMessage($"The song with id: {songId} already exists").WithStatusCode(StatusCode.BadRequest);

            var track = await GetTrackAsync(songId);
            if (track is null)
                return new Result().WithMessage($"The song with id: {songId} was not found").WithStatusCode(StatusCode.NotFound);

            song = new SpotifySong
            {
                Id = track.Id,
                Name = track.Name
            };

            try
            {
                await _songsStorageContext.Songs.AddAsync(song);
                await _songsStorageContext.SaveChangesAsync();
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Could not add song with id: {SongId} to database", songId);
            }

            return new Result().WithStatusCode(StatusCode.Success);
        }

        public async Task<Result> RemoveSongAsync(string songId)
        {
            if (!HasCorrectId(songId))
                return new Result().WithMessage($"The song with id: {songId} has invalid length").WithStatusCode(StatusCode.BadRequest);

            var songToRemove = await _songsStorageContext.Songs.FirstOrDefaultAsync(song => song.Id == songId);
            if (songToRemove is null)
                return new Result().WithMessage($"The song with id: {songId} does not exist").WithStatusCode(StatusCode.BadRequest);

            try
            {
                _songsStorageContext.Songs.Remove(songToRemove);
                await _songsStorageContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not remove song with id: {SongId}", songId);
            }

            return new Result().WithStatusCode(StatusCode.Success);
        }

        public async Task<Result<IEnumerable<SpotifySong>>> ListAsync()
        {
            var likedSongsResult = new Result<IEnumerable<SpotifySong>>();
            var likedSongs = new List<SpotifySong>();
            var wasAnySongRemoved = false;

            var songs = _songsStorageContext.Songs.AsNoTracking();
            if (!songs.Any())
                return likedSongsResult.WithStatusCode(StatusCode.NotFound);

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
                try
                {
                    await _songsStorageContext.SaveChangesAsync();
                }
                catch (Exception ex) 
                {
                    _logger.LogError(ex, "Could not remove songs from db in method {MethodName}", nameof(ListAsync));
                }
            }

            return likedSongsResult.WithData(songs).WithStatusCode(StatusCode.Success);
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace TechTestBackend.Controllers;

[ApiController]
[Route("api/spotify")]
public class SpotifyController : ControllerBase
{
    private readonly ILogger<SpotifyController> _logger;
    private readonly SongsStorageContext _songsStorageContext;

    public SpotifyController(ILogger<SpotifyController> logger, SongsStorageContext songsStorageContext)
    {
        _logger = logger;
        _songsStorageContext = songsStorageContext;
    }

    [HttpGet]
    [Route("searchTracks")]
    public IActionResult SearchTracks(string trackName)
    {
        try
        {        
            // TODO: Implement this method
            var tracks = SpotifyHelper.GetTracks(trackName);

            return Ok(tracks);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Could not find track with name: {TrackName}", trackName);

            return NotFound();
        }
    }

    [HttpPost]
    [Route("like")]
    public async Task<IActionResult> LikeAsync(string id)
    {
        if (!IsSpotifyIdCorrect(id))
            return BadRequest($"Provided id: {id} has invalid length");

        var track = SpotifyHelper.GetTrack(id);
        if (track is null)
        {
            return NotFound();
        }

        var song = new SpotifySong(); //create new song
        song.Id = id;
        song.Name = track.Name;

        try
        {
            //crashes sometimes for some reason
            // we   have to look into this
            await _songsStorageContext.Songs.AddAsync(song);
            
            await _songsStorageContext.SaveChangesAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Could not like song with id: {id}", id);

            return StatusCode(500);
        }
        
        return Ok();
    }
    
    [HttpPost]
    [Route("removeLike")]
    public async Task<IActionResult> RemoveLikeAsync(string id)
    {
        if (!IsSpotifyIdCorrect(id))
            return BadRequest($"Provided id: {id} has invalid length");
        
        var track = SpotifyHelper.GetTrack(id);
        if (track is null)
        {
            return NotFound();
        }

        try
        {
            _songsStorageContext.Songs.Remove(track); // this is not working every tume
            await _songsStorageContext.SaveChangesAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Could not remove track with id: {Id}", id);

            return StatusCode(500);
        }
        
        return Ok();
    }
    
    [HttpGet]
    [Route("listLiked")]
    public async Task<IActionResult> ListLikedAsync()
    {
        var likedSongs = new List<SpotifySong>();

        var songs = _songsStorageContext.Songs.AsNoTracking();
        if (!songs.Any())
            return Ok();

        var wasAnySongRemoved = false;

        foreach (var song in songs)
        {
            if (song is null)
                continue;

            var track = SpotifyHelper.GetTrack(song.Id);
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

        return Ok(likedSongs);
    }
    
    private bool IsSpotifyIdCorrect(string id)
    {
        return id.Length == 22;
    }
}
using Microsoft.AspNetCore.Mvc;
using TechTestBackend.Services;

namespace TechTestBackend.Controllers;

[ApiController]
[Route("api/spotify")]
public class SpotifyController : ControllerBase
{
    private readonly SpotifyService _spotifyService;

    public SpotifyController(SpotifyService spotifyService)
    {
        _spotifyService = spotifyService;
    }

    [HttpGet]
    [Route("searchTracks")]
    public async Task<IActionResult> SearchTracksAsync(string trackName)
    {
       var tracks = await _spotifyService.GetTracksByNameAsync(trackName);

        return Ok(tracks);
    }

    [HttpPost]
    [Route("like")]
    public async Task<IActionResult> LikeAsync(string id)
    {
       await _spotifyService.AddSongAsync(id);
        
        return Ok();
    }
    
    [HttpPost]
    [Route("removeLike")]
    public async Task<IActionResult> RemoveLikeAsync(string id)
    {
       await _spotifyService.RemoveSongAsync(id);
        
        return Ok();
    }
    
    [HttpGet]
    [Route("listLiked")]
    public async Task<IActionResult> ListLikedAsync()
    {
        var likedSongs = await _spotifyService.ListAsync();

        return Ok(likedSongs);
    }
}
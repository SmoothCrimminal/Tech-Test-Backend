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
        if (tracks is null || !tracks.Any())
            return NotFound("No tracks with provided name were found");

        return Ok(tracks);
    }

    [HttpPost]
    [Route("like")]
    public async Task<IActionResult> LikeAsync(string id)
    {
        var result = await _spotifyService.AddSongAsync(id);
        
        return HandleResult(result);
    }
    
    [HttpPost]
    [Route("removeLike")]
    public async Task<IActionResult> RemoveLikeAsync(string id)
    {
        var result = await _spotifyService.RemoveSongAsync(id);

        return HandleResult(result);
    }
    
    [HttpGet]
    [Route("listLiked")]
    public async Task<IActionResult> ListLikedAsync()
    {
        var result = await _spotifyService.ListAsync();

        return HandleResult(result);
    }

    private IActionResult HandleResult(Result result)
    {
        return result.StatusCode switch
        {
            TechTestBackend.StatusCode.NotFound => NotFound(result.Message),
            TechTestBackend.StatusCode.BadRequest => BadRequest(result.Message),
            TechTestBackend.StatusCode.Success => Ok(),
            _ => Ok(result.Message)
        };
    }

    private IActionResult HandleResult<T>(Result<T> result)
    {
        return result.StatusCode switch
        {
            TechTestBackend.StatusCode.NotFound => NotFound(result.Message),
            TechTestBackend.StatusCode.BadRequest => BadRequest(result.Message),
            TechTestBackend.StatusCode.Success => Ok(result.Payload),
            _ => Ok(result.Message)
        };
    }
}
using Microsoft.AspNetCore.Mvc;

namespace TechTestBackend.Controllers;

[ApiController]
[Route("api/spotify")]
public class SpotifyController : ControllerBase
{
    private readonly ILogger<SpotifyController> _logger;

    public SpotifyController(ILogger<SpotifyController> logger)
    {
        _logger = logger;
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
    public IActionResult Like(string id)
    {
        if (!IsSpotifyIdCorrect(id))
            return BadRequest($"Provided id: {id} has invalid length");

        object storage = HttpContext.RequestServices.GetService(typeof(SongsStorageContext));
        
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
            ((SongsStorageContext)storage).Songs.Add(song);
            
            ((SongsStorageContext)storage).SaveChanges();
        }
        catch (Exception e)
        {
            // not sure if this is the best way to handle this
            return Ok();
        }
        
        return Ok();
    }
    
    [HttpPost]
    [Route("removeLike")]
    public IActionResult RemoveLike(string id)
    {
        if (!IsSpotifyIdCorrect(id))
            return BadRequest($"Provided id: {id} has invalid length");

        object storage = HttpContext.RequestServices.GetService(typeof(SongsStorageContext));
        
        var track = SpotifyHelper.GetTrack(id);
        if(track is null)
        {
            return NotFound();
        }

        try
        {
            ((SongsStorageContext)storage).Songs.Remove(track); // this is not working every tume
            ((SongsStorageContext)storage).SaveChanges();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Could not remove track with id: {id}", id);

            return StatusCode(500);
        }
        
        return Ok();
    }
    
    [HttpGet]
    [Route("listLiked")]
    public IActionResult ListLiked()
    {
        object storage = HttpContext.RequestServices.GetService(typeof(SongsStorageContext));

        int songsnumber = ((SongsStorageContext)storage).Songs.Count();
        List<SpotifySong> songs = new List<SpotifySong>(); //((SongstorageContext)storage).Songs.ToList();

        if (songsnumber > 0)
        {
            for (int i = 0; i <= songsnumber - 1; i++)
            {
                string songid = ((SongsStorageContext)HttpContext.RequestServices.GetService(typeof(SongsStorageContext))).Songs.ToList()[i].Id;
            
                var track = SpotifyHelper.GetTrack(songid);
                if(track.Id == null)
                {
                    // TODO: remove song from database, but not sure how
                }
                else
                {
                    // not working for some reason so we have to do the check manually for now
                    // if(SongExists(track.Id) == false)
                    
                    int numerofsong = songs.Count();
                    for (int num = 0; num <= numerofsong; num++)
                    {
                        try
                        {
                            if(songs[num].Id == songid)
                            {
                                break;
                            }
                            else if(num == numerofsong - 1)
                            {

                                for (int namenum = 0; namenum < numerofsong; namenum++)
                                {
                                    if(songs[namenum].Name == track.Name)
                                    {
                                        break; // we dont want to add the same song twice
                                        //does this break work?
                                    }
                                    else if(namenum == numerofsong - 1)
                                    {
                                        songs.Add(((SongsStorageContext)storage).Songs.ToList()[i]);
                                    }
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            // something went wrong, but it's not important
                            songs.Add(((SongsStorageContext)storage).Songs.ToList()[i]);
                        }
                    }
                }
            }
        }

        return Ok(songs);
    }
    
    private bool SongExists(string id)
    {
        return ((SongsStorageContext)HttpContext.RequestServices.GetService(typeof(SongsStorageContext))).Songs.First(e => e.Id == id) != null;
    }
    
    private bool IsSpotifyIdCorrect(string id)
    {
        return id.Length == 22;
    }
}
using TechTestBackend.Dtos;

namespace TechTestBackend.Interfaces
{
    public interface ITracksService
    {
        Task<IEnumerable<SpotifySongDto>?> GetTracksAsync(string trackName);
        Task<SpotifySongDto?> GetTrackAsync(string id);
    }
}

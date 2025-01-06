using TechTestBackend.Dtos;

namespace TechTestBackend.Interfaces
{
    public interface IRemoteTracksService
    {
        Task<IEnumerable<SpotifySongDto>?> GetTracksByNameAsync(string trackName);
        Task<SpotifySongDto?> GetTrackAsync(string id);
    }
}

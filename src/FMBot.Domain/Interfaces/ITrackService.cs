using System.Collections.Generic;
using System.Threading.Tasks;
using FMBot.Domain.Models;

namespace FMBot.Domain.Interfaces
{
    public interface ITrackService
    {
        Task<MusicServiceResponse<IReadOnlyList<Track>>> GetRecentTracks(string username, int amount = 10);

        Task<MusicServiceResponse<IReadOnlyList<Track>>> GetTopTracks(string username, ChartTimePeriod timePeriod);

        Task<MusicServiceResponse<Track>> GetTrack(string name, string artistName, string username = null);

        Task<MusicServiceResponse<Track>> SearchTrack(string query, string username = null);
    }
}

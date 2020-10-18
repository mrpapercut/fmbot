using System.Collections.Generic;
using System.Threading.Tasks;
using FMBot.Domain.Models;

namespace FMBot.Domain.Interfaces
{
    public interface IArtistService
    {
        Task<MusicServiceResponse<IReadOnlyList<Artist>>> GetTopArtists(string username, ChartTimePeriod timePeriod);

        Task<MusicServiceResponse<Artist>> GetArtist(string name, string username = null);

        Task<MusicServiceResponse<Artist>> SearchArtist(string query, string username = null);
    }
}

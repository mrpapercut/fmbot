using System.Collections.Generic;
using System.Threading.Tasks;
using FMBot.Domain.Models;

namespace FMBot.Domain.Interfaces
{
    public interface IAlbumService
    {
        Task<MusicServiceResponse<IReadOnlyList<Album>>> GetTopAlbums(string username, ChartTimePeriod timePeriod);

        Task<MusicServiceResponse<Album>> GetAlbum(string name, string artistName, string username = null);

        Task<MusicServiceResponse<Album>> SearchAlbum(string query, string username = null);
    }
}

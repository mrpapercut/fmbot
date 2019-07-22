using Bot.Domain.LastFM;
using System.Threading.Tasks;

namespace Bot.LastFM.Interfaces.Services
{
    public interface IAlbumInformation
    {
        /// <summary>
        /// Get album info
        /// </summary>
        /// <param name="artist">The artist on the album.</param>
        /// <param name="album">The album name.</param>
        /// <returns>An album <see cref="Album"/>.</returns>
        Task<Album> GetAlbumInfoAsync(string artist, string album);
    }
}

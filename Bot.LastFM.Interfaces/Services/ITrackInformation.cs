using Bot.Persistence.Domain.LastFM;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bot.LastFM.Interfaces.Services
{
    public interface ITrackInformation
    {
        /// <summary>
        /// Get recent tracks
        /// </summary>
        /// <param name="lastFMUserName">The Id of the client.</param>
        /// <param name="amountOfTracks">The total amount of shards the client has.</param>
        /// <returns>An list of tracks <see cref="IReadOnlyList"/>.</returns>
        Task<IReadOnlyList<Track>> GetRecentTracksAsync(string lastFMUserName, int amountOfTracks);
    }
}
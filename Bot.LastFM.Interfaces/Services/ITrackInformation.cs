using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bot.Domain.LastFM;

namespace Bot.LastFM.Interfaces.Services
{
    public interface ITrackInformation : IDisposable
    {
        /// <summary>
        /// Get recent tracks
        /// </summary>
        /// <param name="lastFMUserName">The Last.FM username you wish to retrieve tracks from.</param>
        /// <param name="amountOfTracks">The total amount of tracks you wish to retrieve.</param>
        /// <returns>An list of tracks <see cref="IReadOnlyList"/>.</returns>
        Task<IReadOnlyList<Track>> GetRecentTracksAsync(string lastFMUserName, int amountOfTracks = 2);
    }
}
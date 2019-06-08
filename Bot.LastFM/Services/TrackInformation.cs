using Bot.LastFM.Configurations;
using Bot.LastFM.Interfaces.Services;
using Bot.Persistence.Domain.LastFM;
using IF.Lastfm.Core.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bot.LastFM.Services
{
    public class TrackInformation : ITrackInformation
    {
        private readonly LastfmClient _fmClient = new LastfmClient(RestClientsConfig.TokenConfig.LastFMKey, RestClientsConfig.TokenConfig.LastFMSecret);

        public async Task<IReadOnlyList<Track>> GetRecentTracksAsync(string lastFMUserName, int amountOfTracks)
        {
            var tracks = await _fmClient.User.GetRecentScrobbles(lastFMUserName, null, 1, amountOfTracks).ConfigureAwait(false);

            return tracks.SelectMany(s => new List<Track>
            {
               AlbumName
            });
        }
    }
}
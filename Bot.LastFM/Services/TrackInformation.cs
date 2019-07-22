using Bot.LastFM.Configurations;
using Bot.LastFM.Interfaces.Services;
using IF.Lastfm.Core.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bot.Domain.LastFM;
using IF.Lastfm.Core.Api.Helpers;
using IF.Lastfm.Core.Objects;

namespace Bot.LastFM.Services
{
    public class TrackInformation : ITrackInformation
    {
        private readonly LastfmClient _fmClient = new LastfmClient(RestClientsConfig.TokenConfig.LastFMKey, RestClientsConfig.TokenConfig.LastFMSecret);

        public async Task<IReadOnlyList<Track>> GetRecentTracksAsync(string lastFMUserName, int amountOfTracks = 2)
        {
            var tracks = await this._fmClient.User.GetRecentScrobbles(lastFMUserName, null, 1, amountOfTracks).ConfigureAwait(false);

            return tracks.Select(s => new Track
            {
               AlbumName = s.AlbumName,
               ArtistMbid = s.ArtistMbid,
               ArtistName = s.ArtistName,
               ArtistUrl = s.Url,
               Duration = s.Duration,
               Id = s.Id,
               Images = new Image
               {
                   Small = s.Images?.Small,
                   Medium = s.Images?.Medium,
                   Large = s.Images?.Large
               },
               IsLoved = s.IsLoved,
               IsNowPlaying = s.IsNowPlaying,
               ListenerCount = s.ListenerCount,
               Rank = s.Rank,
               TimePlayed = s.TimePlayed,
               Url = s.Url,
               UserPlayCount = s.UserPlayCount,
               PlayCount = s.PlayCount,
               Name = s.Name,
               Mbid = s.Mbid
            }).ToList();
        }

        public void Dispose()
        {
            this._fmClient?.Dispose();
        }
    }
}

using Bot.Domain.LastFM;
using Bot.LastFM.Configurations;
using Bot.LastFM.Interfaces.Services;
using IF.Lastfm.Core.Api;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bot.Logger.Interfaces;

namespace Bot.LastFM.Services
{
    public class TrackInformation : ITrackInformation
    {
        private readonly LastfmClient _fmClient = new LastfmClient(RestClientsConfig.TokenConfig.LastFMKey, RestClientsConfig.TokenConfig.LastFMSecret);

        private readonly ILogger _logger;

        public TrackInformation(ILogger logger)
        {
            this._logger = logger;
        }

        public async Task<IReadOnlyList<Track>> GetRecentTracksAsync(string lastFMUserName, int amountOfTracks = 2)
        {
            var tracksResult = await this._fmClient.User.GetRecentScrobbles(lastFMUserName, null, 1, amountOfTracks).ConfigureAwait(false);

            if (!tracksResult.Success)
            {
                this._logger.Log("Error", $"Last.FM error for GetRecentTracksAsync: {tracksResult.Status}. Query: {lastFMUserName} - {amountOfTracks}");
                return null;
            }

            return tracksResult.Select(s => new Track
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
    }
}

using Bot.Domain.LastFM;
using Bot.LastFM.Configurations;
using Bot.LastFM.Interfaces.Services;
using Bot.Logger.Interfaces;
using IF.Lastfm.Core.Api;
using System.Linq;
using System.Threading.Tasks;

namespace Bot.LastFM.Services
{
    public class AlbumInformation : IAlbumInformation
    {
        private readonly LastfmClient _fmClient = new LastfmClient(RestClientsConfig.TokenConfig.LastFMKey, RestClientsConfig.TokenConfig.LastFMSecret);
        private readonly ILogger _logger;

        public AlbumInformation(ILogger logger)
        {
            this._logger = logger;
        }

        public async Task<Album> GetAlbumInfoAsync(string artist, string album)
        {
            var albumResult = await this._fmClient.Album.GetInfoAsync(artist, album).ConfigureAwait(false);

            if (!albumResult.Success)
            {
                this._logger.Log("Error", $"Last.FM error for GetAlbumInfo: {albumResult.Status}. Query: {artist} - {album}");
                return null;
            }

            return new Album
            {
                Id = albumResult.Content.Id,
                Name = albumResult.Content.Name,
                Tracks = albumResult.Content.Tracks.Select(s => new Track
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
                }),
                ArtistName = albumResult.Content.ArtistName,
                ReleaseDateUtc = albumResult.Content.ReleaseDateUtc,
                ListenerCount = albumResult.Content.ListenerCount,
                PlayCount = albumResult.Content.PlayCount,
                Mbid = albumResult.Content.Mbid,
                Url = albumResult.Content.Url,
                Images = new Image
                {
                    Large = albumResult.Content.Images.Large,
                    Medium = albumResult.Content.Images.Medium,
                    Small = albumResult.Content.Images.Small
                }
            };
        }
    }
}

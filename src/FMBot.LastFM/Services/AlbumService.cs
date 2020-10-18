using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FMBot.Domain.Interfaces;
using FMBot.Domain.Models;
using FMBot.LastFM.Domain.Models;
using FMBot.LastFM.Domain.Types;
using Album = FMBot.Domain.Models.Album;
using Track = FMBot.Domain.Models.Track;
using Image = FMBot.Domain.Models.Image;
using Tag = FMBot.Domain.Models.Tag;

namespace FMBot.LastFM.Services
{
    public class AlbumService : IAlbumService
    {
        private readonly ILastfmApi _lastFmApi;

        public AlbumService(ILastfmApi lastFmApi)
        {
            this._lastFmApi = lastFmApi;
        }

        public async Task<MusicServiceResponse<IReadOnlyList<Album>>> GetTopAlbums(string username, ChartTimePeriod timePeriod)
        {
            throw new System.NotImplementedException();
        }

        public async Task<MusicServiceResponse<Album>> GetAlbum(string name, string artistName, string username = null)
        {
            var queryParams = new Dictionary<string, string>
            {
                {"artist", artistName },
                {"album", name },
                {"username", username }
            };

            var albumCall = await this._lastFmApi.CallApiAsync<AlbumResponse>(queryParams, Call.AlbumInfo);

            if (albumCall.Success)
            {
                return new MusicServiceResponse<Album>
                {
                    Content = new Album
                    {
                        Name = albumCall.Content.Album.Name,
                        Description = albumCall.Content.Album.Wiki.Content,
                        MbId = albumCall.Content.Album.Mbid,
                        Listeners = albumCall.Content.Album.Listeners,
                        TotalPlaycount = albumCall.Content.Album.Playcount,
                        Url = albumCall.Content.Album.Url,
                        Tags = albumCall.Content.Album.Tags?.Tag?.Select(s => new Tag(s.Name, s.Url)).ToList(),
                        UserPlaycount = albumCall.Content.Album.Userplaycount,
                        Images = albumCall.Content.Album?.Image?.Select(s => new Image(s.Text, s.Size)).ToList(),
                        Tracks = albumCall.Content.Album.Tracks?.Track?.Select(s => new Track
                        {
                            Name = s.Name,
                            Url = s.Url,
                            Duration = s.Duration
                        }).ToList()
                    },
                    Success = true
                };
            }

            return new MusicServiceResponse<Album>
            {
                Message = albumCall.Message,
                Success = false
            };
        }

        public async Task<MusicServiceResponse<Album>> SearchAlbum(string query, string username = null)
        {
            throw new System.NotImplementedException();
        }
    }
}

using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using FMBot.Bot.Extensions;
using FMBot.Bot.Interfaces;
using FMBot.Bot.Models;
using FMBot.Bot.Services;
using FMBot.Bot.Services.Guild;
using FMBot.Bot.Services.ThirdParty;
using FMBot.Bot.Services.WhoKnows;
using FMBot.Domain.Models;
using FMBot.Persistence.Domain.Models;

namespace FMBot.Bot.Builders;

public class AlbumBuilders
{
    private readonly UserService _userService;
    private readonly GuildService _guildService;
    private readonly AlbumService _albumService;
    private readonly WhoKnowsAlbumService _whoKnowsAlbumService;
    private readonly PlayService _playService;
    private readonly SpotifyService _spotifyService;
    private readonly TrackService _trackService;
    private readonly TimeService _timeService;
    private readonly CensorService _censorService;
    private readonly IUpdateService _updateService;

    public AlbumBuilders(UserService userService, GuildService guildService, AlbumService albumService, WhoKnowsAlbumService whoKnowsAlbumService, PlayService playService, SpotifyService spotifyService, TrackService trackService, IUpdateService updateService, TimeService timeService, CensorService censorService)
    {
        this._userService = userService;
        this._guildService = guildService;
        this._albumService = albumService;
        this._whoKnowsAlbumService = whoKnowsAlbumService;
        this._playService = playService;
        this._spotifyService = spotifyService;
        this._trackService = trackService;
        this._updateService = updateService;
        this._timeService = timeService;
        this._censorService = censorService;
    }

    public async Task<ResponseModel> AlbumAsync(
        string prfx,
        IGuild discordGuild,
        IChannel discordChannel,
        IUser discordUser,
        User contextUser,
        string searchValue)
    {
        var response = new ResponseModel
        {
            ResponseType = ResponseType.Embed,
        };

        var albumSearch = await this._albumService.SearchAlbum(response, discordUser, searchValue, contextUser.UserNameLastFM, contextUser.SessionKeyLastFm);
        if (albumSearch.Album == null)
        {
            return albumSearch.Response;
        }

        var databaseAlbum = await this._spotifyService.GetOrStoreSpotifyAlbumAsync(albumSearch.Album);
        databaseAlbum.Tracks = await this._spotifyService.GetExistingAlbumTracks(databaseAlbum.Id);

        var userTitle = await this._userService.GetUserTitleAsync(discordGuild, discordUser);
        response.EmbedAuthor.WithName(
            StringExtensions.TruncateLongString($"Info about {albumSearch.Album.ArtistName} - {albumSearch.Album.AlbumName} for {userTitle}", 255));

        if (albumSearch.Album.AlbumUrl != null)
        {
            response.Embed.WithUrl(albumSearch.Album.AlbumUrl);
        }

        response.Embed.WithAuthor(response.EmbedAuthor);

        if (databaseAlbum.ReleaseDate != null)
        {
            response.Embed.WithDescription($"Release date: `{databaseAlbum.ReleaseDate}`");
        }

        var artistUserTracks = await this._trackService.GetArtistUserTracks(contextUser.UserId, albumSearch.Album.ArtistName);

        var globalStats = new StringBuilder();
        globalStats.AppendLine($"`{albumSearch.Album.TotalListeners}` {StringExtensions.GetListenersString(albumSearch.Album.TotalListeners)}");
        globalStats.AppendLine($"`{albumSearch.Album.TotalPlaycount}` global {StringExtensions.GetPlaysString(albumSearch.Album.TotalPlaycount)}");
        if (albumSearch.Album.UserPlaycount.HasValue)
        {
            globalStats.AppendLine($"`{albumSearch.Album.UserPlaycount}` {StringExtensions.GetPlaysString(albumSearch.Album.UserPlaycount)} by you");
            globalStats.AppendLine($"`{await this._playService.GetWeekAlbumPlaycountAsync(contextUser.UserId, albumSearch.Album.AlbumName, albumSearch.Album.ArtistName)}` by you last week");
            await this._updateService.CorrectUserAlbumPlaycount(contextUser.UserId, albumSearch.Album.ArtistName,
                albumSearch.Album.AlbumName, albumSearch.Album.UserPlaycount.Value);
        }

        if (albumSearch.Album.UserPlaycount.HasValue && albumSearch.Album.AlbumTracks != null && albumSearch.Album.AlbumTracks.Any() && artistUserTracks.Any())
        {
            var listeningTime = await this._timeService.GetPlayTimeForAlbum(albumSearch.Album.AlbumTracks, artistUserTracks,
                albumSearch.Album.UserPlaycount.Value);
            globalStats.AppendLine($"`{StringExtensions.GetListeningTimeString(listeningTime)}` spent listening");
        }

        response.Embed.AddField("Statistics", globalStats.ToString(), true);

        if (discordGuild != null)
        {
            var serverStats = "";
            var guild = await this._guildService.GetGuildForWhoKnows(discordGuild.Id);

            if (guild?.LastIndexed != null)
            {
                var usersWithAlbum = await this._whoKnowsAlbumService.GetIndexedUsersForAlbum(discordGuild, guild.GuildId, albumSearch.Album.ArtistName, albumSearch.Album.AlbumName);
                var filteredUsersWithAlbum = WhoKnowsService.FilterGuildUsersAsync(usersWithAlbum, guild);

                if (filteredUsersWithAlbum.Count != 0)
                {
                    var serverListeners = filteredUsersWithAlbum.Count;
                    var serverPlaycount = filteredUsersWithAlbum.Sum(a => a.Playcount);
                    var avgServerPlaycount = filteredUsersWithAlbum.Average(a => a.Playcount);

                    serverStats += $"`{serverListeners}` {StringExtensions.GetListenersString(serverListeners)}";
                    serverStats += $"\n`{serverPlaycount}` total {StringExtensions.GetPlaysString(serverPlaycount)}";
                    serverStats += $"\n`{(int)avgServerPlaycount}` avg {StringExtensions.GetPlaysString((int)avgServerPlaycount)}";
                }
                else
                {
                    serverStats += $"\nNo listeners in this server.";
                }

                if (usersWithAlbum.Count > filteredUsersWithAlbum.Count)
                {
                    var filteredAmount = usersWithAlbum.Count - filteredUsersWithAlbum.Count;
                    serverStats += $"\n`{filteredAmount}` users filtered";
                }
            }
            else
            {
                serverStats += $"Run `{prfx}index` to get server stats";
            }

            response.Embed.AddField("Server stats", serverStats, true);
        }

        var albumCoverUrl = albumSearch.Album.AlbumCoverUrl;
        if (databaseAlbum.SpotifyImageUrl != null)
        {
            albumCoverUrl = databaseAlbum.SpotifyImageUrl;
        }
        if (albumCoverUrl != null)
        {
            var safeForChannel = await this._censorService.IsSafeForChannel(discordGuild, discordChannel,
                albumSearch.Album.AlbumName, albumSearch.Album.ArtistName, albumSearch.Album.AlbumUrl);
            if (safeForChannel.Result)
            {
                response.Embed.WithThumbnailUrl(albumCoverUrl);
            }
        }

        var footer = new StringBuilder();

        if (contextUser.TotalPlaycount.HasValue && albumSearch.Album.UserPlaycount is >= 10)
        {
            footer.AppendLine($"{(decimal)albumSearch.Album.UserPlaycount.Value / contextUser.TotalPlaycount.Value:P} of all your scrobbles are on this album");
        }

        if (databaseAlbum?.Label != null)
        {
            footer.AppendLine($"Label: {databaseAlbum.Label}");
        }

        if (footer.Length > 0)
        {
            response.Embed.WithFooter(footer.ToString());
        }

        if (albumSearch.Album.Description != null)
        {
            response.Embed.AddField("Summary", albumSearch.Album.Description);
        }

        if (albumSearch.Album.AlbumTracks != null && albumSearch.Album.AlbumTracks.Any())
        {
            var trackDescription = new StringBuilder();

            for (var i = 0; i < albumSearch.Album.AlbumTracks.Count; i++)
            {
                var track = albumSearch.Album.AlbumTracks.OrderBy(o => o.Rank).ToList()[i];

                var albumTrackWithPlaycount = artistUserTracks.FirstOrDefault(f =>
                    StringExtensions.SanitizeTrackNameForComparison(track.TrackName)
                        .Equals(StringExtensions.SanitizeTrackNameForComparison(f.Name)));

                trackDescription.Append(
                    $"{i + 1}.");

                trackDescription.Append(
                    $" **{track.TrackName}**");

                if (albumTrackWithPlaycount != null)
                {
                    trackDescription.Append(
                        $" - *{albumTrackWithPlaycount.Playcount} {StringExtensions.GetPlaysString(albumTrackWithPlaycount.Playcount)}*");
                }

                if (track.Duration.HasValue)
                {
                    trackDescription.Append(albumTrackWithPlaycount == null ? " — " : " - ");

                    var duration = TimeSpan.FromSeconds(track.Duration.Value);
                    var formattedTrackLength =
                        $"{(duration.Hours == 0 ? "" : $"{duration.Hours}:")}{duration.Minutes}:{duration.Seconds:D2}";
                    trackDescription.Append($"`{formattedTrackLength}`");
                }

                trackDescription.AppendLine();


                if (trackDescription.Length > 900 && (albumSearch.Album.AlbumTracks.Count - 2 - i) > 1)
                {
                    trackDescription.Append($"*And {albumSearch.Album.AlbumTracks.Count - 2 - i} more tracks (view all with `{prfx}albumtracks`)*");
                    break;
                }
            }
            response.Embed.AddField("Tracks", trackDescription.ToString());
        }

        //if (album.Tags != null && album.Tags.Any())
        //{
        //    var tags = LastFmRepository.TagsToLinkedString(album.Tags);

        //    response.Embed.AddField("Tags", tags);
        //}

        response.Embed.WithFooter(footer.ToString());
        return response;
    }
}

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bot.Discord.Helpers;
using Bot.Domain.Enums;
using Bot.Domain.LastFM;
using Bot.Domain.Persistence;
using Bot.LastFM.Interfaces.Services;
using Bot.LastFM.Services;
using Bot.Logger.Interfaces;
using Bot.Persistence.UnitOfWorks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace Bot.Discord.Commands.LastFMCommands
{
    [Name("Tracks")]
    public class TrackCommands : ModuleBase<SocketCommandContext>
    {
        private readonly ILogger _logger;
        private readonly DiscordShardedClient _shardedClient;
        private readonly EmbedBuilder _embed;
        private readonly EmbedAuthorBuilder _embedAuthor;
        private readonly EmbedFooterBuilder _embedFooter;

        private User _user;
        private IReadOnlyList<Track> _tracks;

        public TrackCommands(ILogger logger, DiscordShardedClient shardedClient)
        {
            _logger = logger;
            _shardedClient = shardedClient;
            _embed = new EmbedBuilder();
            _embedAuthor = new EmbedAuthorBuilder();
            _embedFooter = new EmbedFooterBuilder();
        }


        /// <summary>
        /// Gets last track for a user
        /// </summary>
        [Command("fm", RunMode = RunMode.Async)]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        public async Task FMAsync()
        {
            _user = await Unity.Resolve<IUnitOfWork>().Users.GetUserAsync(Context.User.Id).ConfigureAwait(false);

            if (_user?.LastFMUserName == null)
            {
                _embed.WithTitle("Error while attempting get latest tracks");
                _embed.WithDescription("Last.FM username has not been set");
                _embed.WithColor(Constants.WarningColorOrange);
                await ReplyAsync("", false, _embed.Build()).ConfigureAwait(false);
                return;
            }

            _tracks = await Unity.Resolve<ITrackInformation>().GetRecentTracksAsync(_user.LastFMUserName);

            if (_tracks == null || !_tracks.Any())
            {
                _embed.WithTitle("Error while attempting get latest tracks");
                _embed.WithDescription($"No scrobbles were found on your profile ({_user.LastFMUserName})");
                _embed.WithColor(Constants.WarningColorOrange);
                await ReplyAsync("", false, _embed.Build()).ConfigureAwait(false);
                return;
            }

            _embed.AddField(
                $"Current: {_tracks[0].Name}",
                $"**{_tracks[0].ArtistName}**" + (string.IsNullOrEmpty(_tracks[0].AlbumName) ? "" : $" | {_tracks[0].AlbumName}"));

            if (_user.DefaultFMType == FMType.EmbedFull)
            {
                _embedAuthor.WithName("Last tracks for " + _user.LastFMUserName);
                _embed.AddField(
                    $"Previous: {_tracks[1].Name}",
                    $"**{_tracks[1].ArtistName}**" + (string.IsNullOrEmpty(_tracks[1].AlbumName) ? "" : $" | {_tracks[1].AlbumName}"));
            }
            else
            {
                _embedAuthor.WithName("Last track for " + _user.LastFMUserName);
            }

            _embed.WithTitle(_tracks[0].IsNowPlaying == true
                ? "*Now playing*"
                : $"Last scrobble {_tracks[0].TimePlayed?.ToString("R")}");

            _embedAuthor.WithIconUrl(Context.User.GetAvatarUrl());
            _embed.WithAuthor(_embedAuthor);

            _embed.WithUrl("https://www.last.fm/user/" + _user.LastFMUserName);

            _embed.WithColor(Constants.LastFMColorRed);
            await ReplyAsync("", false, _embed.Build()).ConfigureAwait(false);

            _logger.LogCommandUsed(Context.Guild?.Id, Context.Client.ShardId, Context.Channel.Id, Context.User.Id, "FM");
        }
    }
}
using Bot.Domain.Enums;
using Bot.Domain.LastFM;
using Bot.LastFM.Interfaces.Services;
using Bot.Logger.Interfaces;
using Bot.Persistence.UnitOfWorks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using User = Bot.Domain.Persistence.User;

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
            this._logger = logger;
            this._shardedClient = shardedClient;
            this._embed = new EmbedBuilder();
            this._embedAuthor = new EmbedAuthorBuilder();
            this._embedFooter = new EmbedFooterBuilder();
        }

        /// <summary>
        /// Gets last track for a user
        /// </summary>
        [Command("fm", RunMode = RunMode.Async)]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        public async Task FMAsync()
        {
            this._user = await Unity.Resolve<IUnitOfWork>().Users.GetUserAsync(Context.User.Id).ConfigureAwait(false);

            if (this._user?.LastFMUserName == null)
            {
                await UsernameNotSetAsync();
                return;
            }

            this._tracks = await Unity.Resolve<ITrackInformation>().GetRecentTracksAsync(this._user.LastFMUserName);

            if (this._tracks == null || !this._tracks.Any())
            {
                this._embed.WithTitle("Error while attempting get latest tracks");
                this._embed.WithDescription($"No scrobbles were found on your profile ({this._user.LastFMUserName})");
                this._embed.WithColor(Constants.WarningColorOrange);
                await ReplyAsync("", false, this._embed.Build()).ConfigureAwait(false);
                return;
            }

            this._embed.AddField(
                $"Current: {this._tracks[0].Name}",
                $"By **{this._tracks[0].ArtistName}**" + (string.IsNullOrEmpty(this._tracks[0].AlbumName) ? "" : $" | {this._tracks[0].AlbumName}"));

            if (this._user.DefaultFMType == FMType.EmbedFull)
            {
                this._embedAuthor.WithName("Last tracks for " + this._user.LastFMUserName);
                this._embed.AddField(
                    $"Previous: {this._tracks[1].Name}",
                    $"By **{this._tracks[1].ArtistName}**" + (string.IsNullOrEmpty(this._tracks[1].AlbumName) ? "" : $" | {this._tracks[1].AlbumName}"));
            }
            else
            {
                this._embedAuthor.WithName("Last track for " + this._user.LastFMUserName);
            }

            this._embed.WithTitle(this._tracks[0].IsNowPlaying == true
                ? "*Now playing*"
                : $"Last scrobble {this._tracks[0].TimePlayed?.ToString("R")}");

            this._embedAuthor.WithIconUrl(Context.User.GetAvatarUrl());
            this._embed.WithAuthor(this._embedAuthor);
            this._embed.WithUrl("https://www.last.fm/user/" + this._user.LastFMUserName);

            var fmUser = await Unity.Resolve<IUserInformation>().GetUserInfoAsync(this._user.LastFMUserName);

            this._embedFooter.WithText($"{fmUser.Name} has {fmUser.Playcount} scrobbles.");
            this._embed.WithFooter(this._embedFooter);

            this._embed.WithColor(Constants.LastFMColorRed);
            await ReplyAsync("", false, this._embed.Build()).ConfigureAwait(false);

            this._logger.LogCommandUsed(Context.Guild?.Id, Context.Client.ShardId, Context.Channel.Id, Context.User.Id, "FM");
        }

        private async Task UsernameNotSetAsync()
        {
            this._embed.WithTitle("Error while attempting get latest tracks");
            this._embed.WithDescription("Last.FM username has not been set. \n" +
                                        "To setup your Last.FM account with this bot, please use the `.fmset username` command.");
            this._embed.WithColor(Constants.WarningColorOrange);
            await ReplyAsync("", false, this._embed.Build()).ConfigureAwait(false);
        }
    }
}

using System.Linq;
using System.Threading.Tasks;
using Bot.Discord.Helpers;
using Bot.Domain.Enums;
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

        private readonly ITrackInformation _trackInformation;

        public TrackCommands(ILogger logger, DiscordShardedClient shardedClient, ITrackInformation trackInformation)
        {
            _logger = logger;
            _shardedClient = shardedClient;
            _embed = new EmbedBuilder();
            _trackInformation = trackInformation;
        }


        /// <summary>
        /// Sends the ping of the shard that is connected to the server where the command is requested.
        /// </summary>
        [Command("fm", RunMode = RunMode.Async)]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        public async Task FMAsync()
        {
            using (var unitOfWork = Unity.Resolve<IUnitOfWork>())
            {
                var user = await unitOfWork.Users.GetUserAsync(Context.User.Id).ConfigureAwait(false);

                if (user?.LastFMUserName == null)
                {
                    _embed.WithTitle("Error while attempting get latest tracks");
                    _embed.WithDescription("Last.FM username has not been set");
                    _embed.WithColor(Constants.WarningColorOrange);
                    await ReplyAsync("", false, _embed.Build()).ConfigureAwait(false);
                    return;
                }

                var tracks = await _trackInformation.GetRecentTracksAsync(user.LastFMUserName);

                if (user?.LastFMUserName == null)
                {
                    _embed.WithTitle("Error while attempting get latest tracks");
                    _embed.WithDescription($"No scrobbles were found on your profile ({user.LastFMUserName})");
                    _embed.WithColor(Constants.WarningColorOrange);
                    await ReplyAsync("", false, _embed.Build()).ConfigureAwait(false);
                    return;
                }

                _embed.AddField(
                    $"Current: {tracks[0].Name}", 
                    $"**{tracks[0].ArtistName}**" + (string.IsNullOrEmpty(tracks[0].AlbumName) ? "" : $" | {tracks[0].AlbumName}"));

                if (user.DefaultFMType == FMType.EmbedFull)
                {
                    _embed.AddField(
                        $"Previous: {tracks[1].Name}",
                        $"**{tracks[1].ArtistName}**" + (string.IsNullOrEmpty(tracks[1].AlbumName) ? "" : $" | {tracks[1].AlbumName}"));
                }

                _embed.WithTitle("Info for " + Context.User.Username);
                _embed.WithUrl("https://www.last.fm/user/" + user.LastFMUserName);
                _embed.WithDescription($"{Context.Client.Latency} ms");
                _embed.WithColor(Constants.LastFMColorRed);
                await ReplyAsync("", false, _embed.Build()).ConfigureAwait(false);
                _logger.LogCommandUsed(Context.Guild?.Id, Context.Client.ShardId, Context.Channel.Id, Context.User.Id, "FM");
            }
        }
    }
}
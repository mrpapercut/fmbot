using System.Linq;
using System.Threading.Tasks;
using Bot.Discord.Helpers;
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

        public TrackCommands(ILogger logger, DiscordShardedClient shardedClient)
        {
            _logger = logger;
            _shardedClient = shardedClient;
            _embed = new EmbedBuilder();
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

                if (user == null || user.LastFMUserName == null)
                {
                    _embed.WithTitle("Error while attempting get latest tracks");
                    _embed.WithDescription("Last.FM username has not been set");
                    _embed.WithColor(Constants.WarningColorOrange);
                    await ReplyAsync("", false, _embed.Build()).ConfigureAwait(false);
                    return;
                }


                _embed.WithTitle("Info for " + Context.User.Username);
                _embed.WithDescription($"{Context.Client.Latency} ms");
                _embed.WithColor(Constants.LastFMColorRed);
                await ReplyAsync("", false, _embed.Build()).ConfigureAwait(false);
                _logger.LogCommandUsed(Context.Guild?.Id, Context.Client.ShardId, Context.Channel.Id, Context.User.Id, "FM");
            }
        }
    }
}
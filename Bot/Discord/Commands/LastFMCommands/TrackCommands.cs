using System.Linq;
using System.Threading.Tasks;
using Bot.Discord.Helpers;
using Bot.Logger.Interfaces;
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
            _embed.WithTitle("Info for " + Context.User.Username);
            _embed.WithDescription($"{Context.Client.Latency} ms");
            _embed.WithColor(new Color(255, 255, 255));
            await ReplyAsync("", false, _embed.Build()).ConfigureAwait(false);
            _logger.LogCommandUsed(Context.Guild?.Id, Context.Client.ShardId, Context.Channel.Id, Context.User.Id, "Ping");
        }
    }
}

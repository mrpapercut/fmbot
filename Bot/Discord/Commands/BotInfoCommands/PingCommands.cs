using System.Linq;
using System.Threading.Tasks;
using Bot.Discord.Helpers;
using Bot.Logger.Interfaces;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace Bot.Discord.Commands.BotInfoCommands
{
    [Name("Ping")]
    public class PingCommands : ModuleBase<SocketCommandContext>
    {
        private readonly ILogger _logger;
        private readonly DiscordShardedClient _shardedClient;
        private readonly EmbedBuilder _embed;

        public PingCommands(ILogger logger, DiscordShardedClient shardedClient)
        {
            this._logger = logger;
            this._shardedClient = shardedClient;
            this._embed = new EmbedBuilder();
        }


        /// <summary>
        /// Sends the ping of the shard that is connected to the server where the command is requested.
        /// </summary>
        [Command("ping", RunMode = RunMode.Async)]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        public async Task PingAsync()
        {
            this._embed.WithTitle("Info for " + Context.User.Username);
            this._embed.WithDescription($"{Context.Client.Latency} ms");
            this._embed.WithColor(new Color(255, 255, 255));
            await ReplyAsync("", false, this._embed.Build()).ConfigureAwait(false);
            this._logger.LogCommandUsed(Context.Guild?.Id, Context.Client.ShardId, Context.Channel.Id, Context.User.Id, "Ping");
        }


        /// <summary>
        /// Sends the ping of all shards to the server where the command is requested.
        /// </summary>
        [Command("shards", RunMode = RunMode.Async)]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        [RequireBotPermission(GuildPermission.SendMessages)]
        public async Task ShardsAsync()
        {
            this._embed.WithTitle("Shard info for " + Context.User.Username);
            foreach (var shard in this._shardedClient.Shards)
            {
                this._embed.AddField($"Shard: {shard.ShardId} {ShardStatusTypeEmoji.GetStatusEmoji(shard.Latency)}", $"{shard.Latency} ms\n" +
                                                                                                                $"{shard.Guilds.Count} Servers\n" +
                                                                                                                $"{shard.Guilds.Sum(x => x.MemberCount)} Members", true);
            }

            this._embed.WithDescription($"Average ping: {this._shardedClient.Shards.Average(x => x.Latency)} ms");
            this._embed.WithFooter($"You are on shard: {Context.Client.ShardId}");
            this._embed.WithColor(new Color(255, 255, 255));
            await ReplyAsync("", false, this._embed.Build()).ConfigureAwait(false);
            this._logger.LogCommandUsed(Context.Guild.Id, Context.Client.ShardId, Context.Channel.Id, Context.User.Id, "Shards");
        }
    }
}

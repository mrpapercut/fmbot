using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using Bot.Logger.Interfaces;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace Bot.Discord.Commands.BotInfoCommands
{
    [Name("BotInfo")]
    public class BotInfoCommand : ModuleBase<SocketCommandContext>
    {
        private readonly ILogger _logger;
        private readonly DiscordShardedClient _shardedClient;
        private readonly EmbedBuilder _embed;
        public BotInfoCommand(ILogger logger, DiscordShardedClient shardedClient)
        {
            this._logger = logger;
            this._shardedClient = shardedClient;
            this._embed = new EmbedBuilder();
        }


        /// <summary>
        /// Sends bot info about the current client.
        /// </summary>
        [Command("BotInfo", RunMode = RunMode.Async)]
        public async Task StatsAsync()
        {
            var ramUsages = Math.Round((decimal)Process.GetCurrentProcess().PrivateMemorySize64 / 1000000000, 2);
            var upTime = DateTime.Now.Subtract(Process.GetCurrentProcess().StartTime);
            var upTimeString = $"{upTime.Days}D:{upTime.Hours}H:{upTime.Minutes}M:{upTime.Seconds}S";
            this._embed.WithTitle("Bot info for " + Context.User.Username);
            this._embed.AddField("Bot:", $"{this._shardedClient.CurrentUser.Username}#{this._shardedClient.CurrentUser.Discriminator}", true);
            this._embed.AddField("Bot id:", this._shardedClient.CurrentUser.Id, true);
            this._embed.AddField("Owner:", Constants.OwnerUsername, true);
            this._embed.AddField("Owner id:", Constants.OwnerId, true);
            this._embed.AddField("RAM Usage:", $"{ramUsages}GB", true);
            this._embed.AddField("Shards:", this._shardedClient.Shards.Count, true);
            this._embed.AddField("Servers:", this._shardedClient.Shards.Sum(x => x.Guilds.Count), true);
            this._embed.AddField("Members:", this._shardedClient.Shards.SelectMany(shard => shard.Guilds).Sum(guild => guild.MemberCount), true);
            this._embed.AddField("Avg ping:", this._shardedClient.Shards.Average(x => x.Latency), true);
            this._embed.AddField("Up time:", upTimeString, true);
            this._embed.AddField("Version:", GetType().Assembly.GetName().Version.ToString(), true);
            this._embed.AddField("Framework:", Assembly.GetEntryAssembly()?.GetCustomAttribute<TargetFrameworkAttribute>()?.FrameworkName, true);
            this._embed.WithColor(new Color(255, 255, 255));
            this._embed.WithCurrentTimestamp();
            await ReplyAsync("", false, this._embed.Build()).ConfigureAwait(false);
            this._logger.LogCommandUsed(Context.Guild?.Id, Context.Client.ShardId, Context.Channel.Id, Context.User.Id, "BotInfo");
        }
    }
}

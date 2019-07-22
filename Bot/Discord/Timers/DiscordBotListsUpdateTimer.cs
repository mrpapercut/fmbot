using System;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Bot.BotLists.Interfaces.Services;
using Discord.WebSocket;

namespace Bot.Discord.Timers
{
    public class DiscordBotListsUpdateTimer
    {
        private readonly DiscordShardedClient _client;
        private readonly IBotListUpdater _botListUpdater;

        public DiscordBotListsUpdateTimer(DiscordShardedClient client, IBotListUpdater botListUpdater)
        {
            this._client = client;
            this._botListUpdater = botListUpdater;
        }

        private Timer _timer;
        internal Task TimerAsync()
        {
            this._timer = new Timer
            {
                Interval = TimeSpan.FromMinutes(Constants.BotListUpdateMinutes).TotalMilliseconds,
                AutoReset = true,
                Enabled = true
            };
            this._timer.Elapsed += TimerElapsed;
            return Task.CompletedTask;
        }


        /// <summary>
        /// Activated when <see cref="_timer"/> is elapsed.
        /// </summary>
        private async void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            await UpdateStatsAsync().ConfigureAwait(false);
        }


        /// <summary>
        /// Updates all discords list stats.
        /// </summary>
        private async Task UpdateStatsAsync()
        {
            var guildCountArray = this._client.Shards.Select(x => x.Guilds.Count).ToArray();
            var shardIdArray = this._client.Shards.Select(x => x.ShardId).ToArray();
            await this._botListUpdater.UpdateBotListStatsAsync(this._client.CurrentUser.Id, this._client.Shards.Count, guildCountArray, shardIdArray).ConfigureAwait(false);
        }
    }
}

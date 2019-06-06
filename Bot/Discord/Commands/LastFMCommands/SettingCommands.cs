using System.Linq;
using System.Threading.Tasks;
using Bot.Discord.Helpers;
using Bot.LastFM.Helpers;
using Bot.Logger.Interfaces;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace Bot.Discord.Commands.LastFMCommands
{
    [Name("Settings")]
    public class SettingCommands : ModuleBase<SocketCommandContext>
    {
        private readonly ILogger _logger;
        private readonly DiscordShardedClient _shardedClient;
        private readonly EmbedBuilder _embed;
        private readonly ValidationHelper _validationHelper;

        public SettingCommands(ILogger logger, DiscordShardedClient shardedClient)
        {
            _logger = logger;
            _shardedClient = shardedClient;
            _embed = new EmbedBuilder();
            _validationHelper = new ValidationHelper();
        }


        /// <summary>
        /// Sends the ping of the shard that is connected to the server where the command is requested.
        /// </summary>
        [Command("set", RunMode = RunMode.Async)]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        public async Task SetAsync(string lastFMUserName)
        {
            if (!await _validationHelper.LastFMUserExistsAsync(lastFMUserName))
            {
                _embed.WithTitle("Error while attempting to set Last.FM username");
                _embed.WithDescription("The username could not be found. Please double check if you spelled it correctly.");
                _embed.WithColor(Constants.WarningColorOrange);
                await ReplyAsync("", false, _embed.Build()).ConfigureAwait(false);
                return;
            }

            _embed.WithTitle("Info for " + Context.User.Username);
            _embed.WithDescription($"{Context.Client.Latency} ms");
            _embed.WithColor(new Color(255, 255, 255));
            await ReplyAsync("", false, _embed.Build()).ConfigureAwait(false);
            _logger.LogCommandUsed(Context.Guild?.Id, Context.Client.ShardId, Context.Channel.Id, Context.User.Id, "FM");
        }
    }
}

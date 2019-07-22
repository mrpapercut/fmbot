using System.Linq;
using System.Threading.Tasks;
using Bot.Discord.Helpers;
using Bot.LastFM.Helpers;
using Bot.Logger.Interfaces;
using Bot.Persistence.UnitOfWorks;
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
            this._logger = logger;
            this._shardedClient = shardedClient;
            this._embed = new EmbedBuilder();
            this._validationHelper = new ValidationHelper();
        }


        /// <summary>
        /// Sends the ping of the shard that is connected to the server where the command is requested.
        /// </summary>
        [Command("set", RunMode = RunMode.Async)]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        public async Task SetAsync(string lastFMUserName)
        {
            if (!await this._validationHelper.LastFMUserExistsAsync(lastFMUserName))
            {
                this._embed.WithTitle("Error while attempting to set Last.FM username");
                this._embed.WithDescription("The username could not be found. Please double check if you spelled it correctly.");
                this._embed.WithColor(Constants.WarningColorOrange);
                await ReplyAsync("", false, this._embed.Build()).ConfigureAwait(false);
                return;
            }

            using (var unitOfWork = Unity.Resolve<IUnitOfWork>())
            {
                await unitOfWork.Users.GetOrAddUserAsync(Context.User.Id, Context.User.Username);
                await unitOfWork.Users.AddOrUpdateLastFMUserNameAsync(Context.User.Id, lastFMUserName).ConfigureAwait(false);
            }

            this._embed.WithTitle("FMBot settings changed");
            this._embed.WithDescription($"Last.FM username for {Context.User.Username} set to {lastFMUserName}");
            this._embed.WithColor(Constants.LastFMColorRed);
            await ReplyAsync("", false, this._embed.Build()).ConfigureAwait(false);

            this._logger.LogCommandUsed(Context.Guild?.Id, Context.Client.ShardId, Context.Channel.Id, Context.User.Id, "set");
        }

        
    }
}

using Bot.Domain.LastFM;
using Bot.LastFM.Configurations;
using Bot.LastFM.Interfaces.Services;
using IF.Lastfm.Core.Api;
using System.Threading.Tasks;
using Bot.Logger.Interfaces;

namespace Bot.LastFM.Services
{
    public class UserInformation : IUserInformation
    {
        private readonly LastfmClient _fmClient = new LastfmClient(RestClientsConfig.TokenConfig.LastFMKey, RestClientsConfig.TokenConfig.LastFMSecret);

        private readonly ILogger _logger;

        public UserInformation(ILogger logger)
        {
            this._logger = logger;
        }

        public async Task<User> GetUserInfoAsync(string lastFMUserName)
        {
            var userResult = await this._fmClient.User.GetInfoAsync(lastFMUserName).ConfigureAwait(false);

            if (!userResult.Success)
            {
                this._logger.Log("Error", $"Last.FM error for GetUserInfoAsync: {userResult.Status}. Query: {lastFMUserName}");
                return null;
            }

            return new User
            {
                Name = userResult.Content.Name,
                FullName = userResult.Content.FullName,
                Avatar = new Image
                {
                    Large = userResult.Content.Avatar.Large,
                    Medium = userResult.Content.Avatar.Medium,
                    Small = userResult.Content.Avatar.Small,
                },
                Id = userResult.Content.Id,
                Age = userResult.Content.Age,
                Country = userResult.Content.Country,
                Gender = userResult.Content.Gender.ToString(),
                IsSubscriber = userResult.Content.IsSubscriber,
                Playcount = userResult.Content.Playcount,
                Playlists = userResult.Content.Playlists,
                TimeRegistered = userResult.Content.TimeRegistered,
                Bootstrap = userResult.Content.Bootstrap,
                Type = userResult.Content.Type
            };
        }
    }
}

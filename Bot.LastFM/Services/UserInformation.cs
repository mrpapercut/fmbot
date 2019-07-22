using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bot.Domain.LastFM;
using Bot.LastFM.Configurations;
using Bot.LastFM.Interfaces.Services;
using IF.Lastfm.Core.Api;

namespace Bot.LastFM.Services
{
    public class UserInformation : IUserInformation
    {
        private readonly LastfmClient _fmClient = new LastfmClient(RestClientsConfig.TokenConfig.LastFMKey, RestClientsConfig.TokenConfig.LastFMSecret);

        public async Task<User> GetUserInfoAsync(string lastFMUserName)
        {
            var user = await this._fmClient.User.GetInfoAsync(lastFMUserName).ConfigureAwait(false);

            return new User()
            {
                Name = user.Content.Name,
                FullName = user.Content.FullName,
                Avatar = new Image
                {
                    Large = user.Content.Avatar.Large,
                    Medium = user.Content.Avatar.Medium,
                    Small = user.Content.Avatar.Small,
                },
                Id = user.Content.Id,
                Age = user.Content.Age,
                Country = user.Content.Country,
                Gender = user.Content.Gender.ToString(),
                IsSubscriber = user.Content.IsSubscriber,
                Playcount = user.Content.Playcount,
                Playlists = user.Content.Playlists,
                TimeRegistered = user.Content.TimeRegistered,
                Bootstrap = user.Content.Bootstrap,
                Type = user.Content.Type
            };
        }

        public void Dispose()
        {
            this._fmClient?.Dispose();
        }
    }
}

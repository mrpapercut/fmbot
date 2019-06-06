using System.Threading.Tasks;
using Bot.LastFM.Configurations;
using IF.Lastfm.Core.Api;

namespace Bot.LastFM.Helpers
{
    public class ValidationHelper
    {
        private readonly LastfmClient _fmClient = new LastfmClient(RestClientsConfig.TokenConfig.LastFMKey, RestClientsConfig.TokenConfig.LastFMSecret);

        public async Task<bool> LastFMUserExistsAsync(string lastFMUserName)
        {
            var lastFMUser = await _fmClient.User.GetInfoAsync(lastFMUserName).ConfigureAwait(false);

            return lastFMUser.Success;
        }
    }
}
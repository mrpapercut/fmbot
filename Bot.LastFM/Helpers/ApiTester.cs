using System;
using System.Threading.Tasks;
using Bot.LastFM.Configurations;
using Bot.Logger.Interfaces;
using IF.Lastfm.Core.Api;

namespace Bot.LastFM.Helpers
{
    public class ApiTester
    {
        private readonly LastfmClient _fmClient = new LastfmClient(RestClientsConfig.TokenConfig.LastFMKey, RestClientsConfig.TokenConfig.LastFMSecret);

        public async Task CheckApiKeyAsync()
        {
            Console.WriteLine("Checking Last.FM API...");
            var lastFMUser = await _fmClient.User.GetInfoAsync("Lastfmsupport").ConfigureAwait(false);

            if (lastFMUser.Status.ToString().Equals("BadApiKey"))
            {
                Console.WriteLine("Warning! Invalid API key for Last.FM! Please set the keys in the LastFMConfig.json or the bot will not work properly.");
            }
        }
    }
}
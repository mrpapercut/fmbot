using System;
using System.Threading;
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
            var lastFMUser = await this._fmClient.User.GetInfoAsync("Lastfmsupport").ConfigureAwait(false);

            if (lastFMUser.Status.ToString().Equals("BadApiKey"))
            {
                Console.WriteLine("Warning! Invalid API key for Last.FM! Please set the keys in the LastFMConfig.json! \n \n" +
                                  "Exiting in 10 seconds...");

                Thread.Sleep(10000);
                Environment.Exit(0);
            }
            else
            {
                Console.WriteLine("Last.FM API test successful.");
            }
        }
    }
}

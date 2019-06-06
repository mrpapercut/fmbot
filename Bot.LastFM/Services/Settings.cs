using System;
using System.Threading.Tasks;
using Bot.LastFM.Interfaces.Services;

namespace Bot.LastFM.Services
{
    public class Settings : ISettings
    {
        public Task AddOrUpdateLastFMUserName(ulong id, string lastFMUsername)
        {
            throw new NotImplementedException();
        }
    }
}

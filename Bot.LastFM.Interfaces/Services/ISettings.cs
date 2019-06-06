using System.Threading.Tasks;

namespace Bot.LastFM.Interfaces.Services
{
    public interface ISettings
    {
        /// <summary>
        /// Updates the Last.FM username for the user
        /// </summary>
        /// <param name="id">The id of the user.</param>
        /// <param name="lastFMUsername">The Last.FM username of the user.</param>
        /// <returns>An awaitable <see cref="Task"/>.</returns>
        Task AddOrUpdateLastFMUserName(ulong id, string lastFMUsername);
    }
}

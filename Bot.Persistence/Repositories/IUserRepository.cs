using System.Threading.Tasks;
using Bot.Persistence.Domain;
using Bot.Persistence.Domain.Entities;

namespace Bot.Persistence.Repositories
{
    public interface IUserRepository : IRepository<User>
    {

        /// <summary>
        /// Gets the user if it exists in the database.
        /// And it adds the user to the database if it doesn't exist.
        /// </summary>
        /// <param name="id">The id of the user.</param>
        /// <param name="userName">The name of the user.</param>
        /// <returns>An awaitable <see cref="Task"/> that returns a <see cref="User"/>.</returns>
        Task<User> GetOrAddUserAsync(ulong id, string userName);

        /// <summary>
        /// Updates the Last.FM username for the user
        /// </summary>
        /// <param name="id">The id of the user.</param>
        /// <param name="lastFMUsername">The Last.FM username of the user.</param>
        /// <returns>An awaitable <see cref="Task"/>.</returns>
        Task AddOrUpdateLastFMUserNameAsync(ulong id, string lastFMUsername);

        /// <summary>
        /// Gets user
        /// </summary>
        /// <param name="id">The id of the user.</param>
        /// <returns>An awaitable <see cref="Task"/>.</returns>
        Task<User> GetUserAsync(ulong id);
    }
}


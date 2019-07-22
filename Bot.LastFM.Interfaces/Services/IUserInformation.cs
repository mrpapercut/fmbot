using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bot.Domain.LastFM;

namespace Bot.LastFM.Interfaces.Services
{
    public interface IUserInformation
    {
        /// <summary>
        /// Get user info
        /// </summary>
        /// <param name="lastFMUserName">The Last.FM username you wish to retrieve tracks from.</param>
        /// <returns>A Last.FM user <see cref="User"/>.</returns>
        Task<User> GetUserInfoAsync(string lastFMUserName);
    }
}

using System;
using System.Collections.Generic;
using Bot.Domain.Enums;

namespace Bot.Domain.Persistence
{
    public class User
    {
        /// <summary>The id of the user.</summary>
        public ulong Id { get; set; }

        /// <summary>The username of the user.</summary>
        public string Name { get; set; }

        /// <summary>The <see cref="DateTime"/> of when the user used a command for the last time.</summary>
        public DateTime CommandUsed { get; set; }

        /// <summary>The amount of times the user was timed out for using commands to fast.</summary>
        public int TotalTimesTimedOut { get; set; }

        /// <summary>The amount of warning the user received when the use is using commands to fast.</summary>
        public int SpamWarning { get; set; }

        /// <summary>The amount of times the user was using commands to fast.</summary>
        public int CommandSpam { get; set; }

        /// <summary>If the user is currently featured.</summary>
        public bool? Featured { get; set; }

        /// <summary>Their Last.FM username</summary>
        public string LastFMUserName { get; set; }

        /// <summary>The default timespan for generated charts</summary>
        public LastFMTimespan DefaultTimeSpan { get; set; }

        /// <summary>The usertype for the user. Some commands are restricted for certain types of users.</summary>
        public UserType UserType { get; set; }

        /// <summary>The default embed for the .fm command.</summary>
        public FMType DefaultFMType { get; set; }

        /// <summary>All the <see cref="Requests"/> that were made by the user.</summary>
        public List<Request> Requests { get; set; }
    }
}

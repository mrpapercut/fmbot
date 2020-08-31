using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using FMBot.Bot.Configurations;
using FMBot.Bot.Models;
using FMBot.Bot.Resources;
using FMBot.Persistence.Domain.Models;
using FMBot.Persistence.EntityFrameWork;
using Microsoft.EntityFrameworkCore;

namespace FMBot.Bot.Services.WhoKnows
{
    public class WhoKnowsService
    {
        public static IList<WhoKnowsObjectWithUser> AddOrReplaceUserToIndexList(IList<WhoKnowsObjectWithUser> users, User userSettings, IGuildUser user, string name, long? playcount)
        {
            var existingRecord = users.FirstOrDefault(f => f.UserId == userSettings.UserId);
            if (existingRecord != null)
            {
                users.Remove(existingRecord);
            }

            var userPlaycount = int.Parse(playcount.GetValueOrDefault(0).ToString());
            if (users.Count != 14)
            {
                users.Add(new WhoKnowsObjectWithUser
                {
                    UserId = userSettings.UserId,
                    Name = name,
                    Playcount = userPlaycount,
                    LastFMUsername = userSettings.UserNameLastFM,
                    DiscordUserId = userSettings.DiscordUserId,
                    DiscordName = user.Nickname ?? user.Username
                });
            }

            return users.OrderByDescending(o => o.Playcount).ToList();
        }

        public static string WhoKnowsListToString(IList<WhoKnowsObjectWithUser> user)
        {
            var reply = "";

            var artistsCount = user.Count;
            if (artistsCount > 14)
            {
                artistsCount = 14;
            }

            for (var index = 0; index < artistsCount; index++)
            {
                var artist = user[index];

                var nameWithLink = NameWithLink(artist);
                var playString = GetPlaysString(artist.Playcount);

                if (index == 0)
                {
                    reply += $"👑  {nameWithLink}";
                }
                else
                {
                    reply += $" {index + 1}.  {nameWithLink} ";
                }

                reply += $"- **{artist.Playcount}** {playString}\n";
            }

            if (user.Count == 1)
            {
                reply += $"\nNobody else has this artist in their top {Constants.ArtistsToIndex} artists.";
            }

            return reply;
        }

        private static string NameWithLink(WhoKnowsObjectWithUser user)
        {
            var discordName = user.DiscordName.Replace("(", "").Replace(")", "").Replace("[", "").Replace("]", "");
            var nameWithLink = $"[{discordName}]({Constants.LastFMUserUrl}{user.LastFMUsername})";
            return nameWithLink;
        }

        private static string GetPlaysString(int playcount)
        {
            return playcount == 1 ? "play" : "plays";
        }
    }
}

using Discord;

namespace Bot
{

    /// <summary>
    /// This class contains all the constant variables for the bot.
    /// </summary>
    public static class Constants
    {

        /// <summary>The prefix that the bot will use for the commands.</summary>
        public const string Prefix = ".fm";


        /// <summary>The username of the owner of the bot.</summary>
        public const string OwnerUsername = "Frikandel#0001";


        /// <summary>The id of the owner of this bot.</summary>
        public const ulong OwnerId = 125740103539621888;


        /// <summary>The id of the client.</summary>
        public const ulong BotId = 398231862276784129;


        /// <summary>The username of the client.</summary>
        public const string BotUsername = "BotTemplate#8315";


        /// <summary>The string for a green status emoji.</summary>
        public const string GreenStatusEmoji = "<:GreenStatus:533010751229526032>";


        /// <summary>The string for a orange status emoji.</summary>
        public const string OrangeStatusEmoji = "<:OrangeStatus:533010753196654600>";


        /// <summary>The string for a red status emoji.</summary>
        public const string RedStatusEmoji = "<:RedStatus:533010751074467851>";


        /// <summary>The Discord color for the red Last.FM color.</summary>
        public static Color LastFMColorRed = new Color(186, 0, 0);

        /// <summary>The Discord color for a warning embed.</summary>
        public static Color WarningColorOrange = new Color(255, 174, 66);


        /// <summary>The id of the support server.</summary>
        public const ulong SupportServerId = 404987247746744320;


        /// <summary>The id of the event log server.</summary>
        public const ulong EventSeverId = SupportServerId;


        /// <summary>The id of the disconnect event channel.</summary>
        public const ulong DisconnectEventChannelId = 405026768857333770;


        /// <summary>The id of the connect event channel.</summary>
        public const ulong ConnectEventChannelId = 405026768857333770;


        /// <summary>The id of the latency updated event channel.</summary>
        public const ulong LatencyUpdatedEventChannelId = 405026768857333770;


        /// <summary>The id of the join guild event channel.</summary>
        public const ulong JoinGuildChannelId = 405026768857333770;


        /// <summary>The number of seconds the user has to wait before using another command.</summary>
        public const int SpamFilterSeconds = 3;


        /// <summary>The amount of times the user has to be blocked byt he spam filter to get timeout.</summary>
        public const int SpamFilterTimeouts = 5;


        /// <summary>The amount of minutes the user will be timeout for if he/she was spamming commands.</summary>
        public const int SpamFilterTimeoutMinutes = 5;


        /// <summary>The amount of minutes there will be between updating the bot lists info.</summary>
        public const int BotListUpdateMinutes = 15;
    }
}

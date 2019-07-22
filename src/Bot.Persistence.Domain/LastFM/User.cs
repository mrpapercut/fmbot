using System;
using System.Collections.Generic;
using System.Text;

namespace Bot.Domain.LastFM
{
    public class User
    {
        public string Name { get; set; }

        public string FullName { get; set; }

        public Image Avatar { get; set; }


        public string Id { get; set; }

        public int Age { get; set; }

        public string Country { get; set; }

        public string Gender { get; set; }

        public bool IsSubscriber { get; set; }

        public int Playcount { get; set; }

        public int Playlists { get; set; }

        public DateTime TimeRegistered { get; set; }

        public int Bootstrap { get; set; }

        public string Type { get; set; }
    }
}

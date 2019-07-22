using System;
using System.Collections.Generic;
using System.Text;

namespace Bot.Domain.LastFM
{
    public class Album
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public IEnumerable<Track> Tracks { get; set; }

        public string ArtistName { get; set; }

        public string ArtistMbid { get; set; }

        public DateTimeOffset? ReleaseDateUtc { get; set; }

        public int? ListenerCount { get; set; }

        public int? PlayCount { get; set; }

        public int? UserPlayCount { get; set; }

        public string Mbid { get; set; }

        public Uri Url { get; set; }

        public Image Images { get; set; }
    }
}

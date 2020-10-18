using System;
using System.Collections.Generic;

namespace FMBot.Domain.Models
{
    public class Track
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public bool NowPlaying { get; set; }

        public bool Favorite { get; set; }

        public string Url { get; set; }

        public string SpotifyId { get; set; }

        public Guid? MbId { get; set; }
        
        public Album Album { get; set; }

        public IList<Artist> Artists { get; set; }

        public IList<Tag> Tags { get; set; }

        public IList<Image> Images { get; set; }

        public long? UserPlaycount { get; set; }

        public long TotalPlaycount { get; set; }

        public long Listeners { get; set; }

        public long? Duration { get; set; }
    }
}

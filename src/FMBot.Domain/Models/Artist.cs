using System;
using System.Collections.Generic;

namespace FMBot.Domain.Models
{
    public class Artist
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string Url { get; set; }

        public string SpotifyId { get; set; }

        public Guid? MbId { get; set; }

        public IList<Album> Albums { get; set; }

        public IList<Tag> Tags { get; set; }

        public IList<Image> Images { get; set; }

        public long? UserPlaycount { get; set; }

        public long TotalPlaycount { get; set; }

        public long Listeners { get; set; }
    }
}

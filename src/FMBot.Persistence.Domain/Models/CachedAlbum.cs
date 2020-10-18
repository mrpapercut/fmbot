using System;
using System.Collections.Generic;

namespace FMBot.Persistence.Domain.Models
{
    public class CachedAlbum
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string ArtistName { get; set; }

        public string LastFmUrl { get; set; }

        public Guid? Mbid { get; set; }

        public string SpotifyImageUrl { get; set; }

        public DateTime? SpotifyImageDate { get; set; }

        public string SpotifyId { get; set; }

        public int? Popularity { get; set; }

        public string Label { get; set; }

        public DateTime? ReleaseDate { get; set; }

        public int ArtistId { get; set; }

        public CachedArtist CachedArtist { get; set; }

        public ICollection<CachedTrack> Tracks { get; set; }
    }
}

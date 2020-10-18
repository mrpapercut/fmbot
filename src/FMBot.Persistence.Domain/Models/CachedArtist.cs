using System;
using System.Collections.Generic;

namespace FMBot.Persistence.Domain.Models
{
    public class CachedArtist
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string LastFmUrl { get; set; }

        public Guid? Mbid { get; set; }

        public string SpotifyImageUrl { get; set; }

        public DateTime? SpotifyImageDate { get; set; }

        public string SpotifyId { get; set; }

        public int? Popularity { get; set; }

        public string[] Aliases { get; set; }

        public ICollection<CachedTrack> Tracks { get; set; }

        public ICollection<CachedAlbum> Albums { get; set; }

        public ICollection<ArtistAlias> ArtistAliases { get; set; }

        public ICollection<ArtistGenre> ArtistGenres { get; set; }
    }
}

namespace FMBot.Persistence.Domain.Models
{
    public class ArtistAlias
    {
        public int Id { get; set; }

        public int ArtistId { get; set; }

        public string Alias { get; set; }

        public bool CorrectsInScrobbles { get; set; }

        public CachedArtist CachedArtist { get; set; }
    }
}

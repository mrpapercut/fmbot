namespace FMBot.Domain.Models
{
    public class Image
    {
        public Image(string url, string size)
        {
            this.Url = url;
            this.Size = size;
        }

        public string Url { get; }

        public string Size { get; }
    }
}

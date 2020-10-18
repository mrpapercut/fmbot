namespace FMBot.Domain.Models
{
    public class Tag
    {
        public Tag(string name, string url)
        {
            this.Name = name;
            this.Url = url;
        }

        public string Name { get; }

        public string Url { get; }
    }
}

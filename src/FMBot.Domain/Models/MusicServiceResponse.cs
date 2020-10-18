namespace FMBot.Domain.Models
{
    public class MusicServiceResponse<T>
    {
        public bool Success { get; set; }

        public string Message { get; set; }

        public T Content { get; set; }
    }
}

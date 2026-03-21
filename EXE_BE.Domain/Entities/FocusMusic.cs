namespace EXE_BE.Domain.Entities
{
    public class FocusMusic : Base
    {
        public Guid MusicId { get; set; }
        public Guid UserId { get; set; }

        public string Title { get; set; } = string.Empty;
        public string Artist { get; set; } = string.Empty;
        public string AudioUrl { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public virtual User? User { get; set; }
    }
}

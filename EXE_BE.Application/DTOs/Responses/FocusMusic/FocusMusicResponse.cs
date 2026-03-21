namespace EXE_BE.Application.DTOs.Responses.FocusMusic
{
    public class FocusMusicResponse
    {
        public Guid MusicId { get; set; }
        public Guid UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Artist { get; set; } = string.Empty;
        public string AudioUrl { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsCurrent { get; set; }
    }
}

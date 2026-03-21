namespace EXE_BE.Application.DTOs.Requests.FocusMusic
{
    public class CreateFocusMusicRequest
    {
        public Guid UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Artist { get; set; } = string.Empty;
        public string AudioUrl { get; set; } = string.Empty;
    }
}

namespace EXE_BE.Application.DTOs.Requests.Gameplay
{
    public class StartFocusSessionRequest
    {
        public Guid UserId { get; set; }
        public Guid TopicId { get; set; }
        public int DurationMinutes { get; set; }
        public Guid? RoomId { get; set; }
    }
}

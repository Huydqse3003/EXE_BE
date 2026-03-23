namespace EXE_BE.Application.DTOs.Responses.Gameplay
{
    public class FocusSessionFlowResponse
    {
        public Guid SessionId { get; set; }
        public Guid UserId { get; set; }
        public Guid TopicId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int DurationMinutes { get; set; }
        public string Status { get; set; } = string.Empty;
        public int EarnedCoins { get; set; }
        public int EarnedExp { get; set; }
        public int UserCoins { get; set; }
        public int UserLevel { get; set; }
    }
}

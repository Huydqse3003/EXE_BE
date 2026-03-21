namespace EXE_BE.Application.DTOs.Responses.UserHabit
{
    public class UserHabitResponse
    {
        public Guid HabitId { get; set; }
        public Guid UserId { get; set; }
        public string HabitType { get; set; } = string.Empty;
        public DateTime EventTime { get; set; }
        public DateTime? LoginTime { get; set; }
        public DateTime? LogoutTime { get; set; }
        public string Details { get; set; } = string.Empty;
        public string? MetadataJson { get; set; }
    }
}

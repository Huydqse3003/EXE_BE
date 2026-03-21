namespace EXE_BE.Application.DTOs.Requests.UserHabit
{
    public class CreateUserHabitRequest
    {
        public Guid UserId { get; set; }
        public string HabitType { get; set; } = string.Empty;
        public DateTime? EventTime { get; set; }
        public DateTime? LoginTime { get; set; }
        public DateTime? LogoutTime { get; set; }
        public string? Details { get; set; }
        public string? MetadataJson { get; set; }
    }
}

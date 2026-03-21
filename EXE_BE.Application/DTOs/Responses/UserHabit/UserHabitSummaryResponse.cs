namespace EXE_BE.Application.DTOs.Responses.UserHabit
{
    public class UserHabitSummaryResponse
    {
        public Guid UserId { get; set; }
        public int TotalHabits { get; set; }
        public int TotalLogins { get; set; }
        public int TotalFocusSessions { get; set; }
        public int TotalPurchases { get; set; }
        public int TotalRoomDecorations { get; set; }
        public DateTime? LastLoginTime { get; set; }
        public DateTime? LastEventTime { get; set; }
    }
}

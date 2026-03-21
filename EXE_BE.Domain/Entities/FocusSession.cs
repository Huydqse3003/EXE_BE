

namespace EXE_BE.Domain.Entities
{
    public class FocusSession : Base
    {
        public Guid SessionId { get; set; }
        public Guid UserId { get; set; }
        public Guid TopicId { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int DurationMinutes { get; set; } // Duration planned or achieved

        // Status of the session (e.g., Completed, Failed/Interrupted)
        public string Status { get; set; } = string.Empty;

        // Rewards for focusing
        public int EarnedExp { get; set; }
        public int EarnedCoins { get; set; }

        public Guid? RoomId { get; set; }

        public virtual User? User { get; set; }
        public virtual Topic? Topic { get; set; }
        public virtual StudyRoom? Room { get; set; }
    }
}

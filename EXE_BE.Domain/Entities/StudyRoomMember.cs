

namespace EXE_BE.Domain.Entities
{
    public class StudyRoomMember : Base
    {
        public Guid RoomId { get; set; }
        public Guid UserId { get; set; }

        public DateTime JoinedAt { get; set; }
        public bool IsActive { get; set; } = true; // Still in the room

        // Roles in the room, e.g., "Owner", "Member"
        public string Role { get; set; } = "Member";

        public virtual StudyRoom? Room { get; set; }
        public virtual User? User { get; set; }
    }
}

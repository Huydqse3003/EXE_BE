
using System.Collections.Generic;

namespace EXE_BE.Domain.Entities
{
    public class StudyRoom : Base
    {
        public Guid RoomId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public int MaxCapacity { get; set; } = 10;
        public bool IsPrivate { get; set; } = false;

        // Passcode for private rooms
        public string Passcode { get; set; } = string.Empty;

        public Guid CreatedByUserId { get; set; }
        public virtual User? CreatedByUser { get; set; }

        public virtual ICollection<StudyRoomMember>? Members { get; set; }
        public virtual ICollection<FocusSession>? FocusSessions { get; set; }
    }
}

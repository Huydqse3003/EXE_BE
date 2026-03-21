
using System.Collections.Generic;

namespace EXE_BE.Domain.Entities
{
    public class Topic : Base
    {
        public Guid TopicId { get; set; }
        public Guid UserId { get; set; }

        // e.g. "Math", "Reading", "Coding"
        public string Name { get; set; } = string.Empty;

        // Helps visual users distinguish topics easily
        public string ColorCode { get; set; } = "#FFFFFF";

        public virtual User? User { get; set; }
        public virtual ICollection<FocusSession>? FocusSessions { get; set; }
    }
}

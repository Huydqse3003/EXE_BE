

namespace EXE_BE.Domain.Entities
{
    public class UserItem : Base
    {
        // Composite Key typically, or a separate Guid if you prefer
        public Guid UserId { get; set; }
        public Guid ItemId { get; set; }

        public DateTime AcquiredDate { get; set; }
        public bool IsEquipped { get; set; } // If the user has equipped this pet/theme/icon

        public virtual User? User { get; set; }
        public virtual GameItem? Item { get; set; }
    }
}

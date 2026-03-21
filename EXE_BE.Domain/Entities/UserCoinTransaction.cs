

namespace EXE_BE.Domain.Entities
{
    public class UserCoinTransaction : Base
    {
        public Guid TransactionId { get; set; }
        public Guid UserId { get; set; }

        public int Amount { get; set; } // +30 (if focused), -50 (if bought an item)
        public string Source { get; set; } = string.Empty; // e.g., "FocusSession", "StorePurchase", "Reward"

        // Optional Link back to what caused the transaction (like SessionId)
        public Guid? ReferenceId { get; set; }

        public DateTime TransactionDate { get; set; }

        public virtual User? User { get; set; }
    }
}

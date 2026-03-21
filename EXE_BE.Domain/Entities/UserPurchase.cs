

namespace EXE_BE.Domain.Entities
{
    public class UserPurchase : Base
    {
        public Guid PurchaseId { get; set; }
        public Guid UserId { get; set; }
        public Guid ItemId { get; set; }

        public DateTime PurchaseDate { get; set; }
        public int CoinsSpent { get; set; }

        public virtual User? User { get; set; }
        public virtual GameItem? Item { get; set; }
    }
}

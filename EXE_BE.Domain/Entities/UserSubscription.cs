

namespace EXE_BE.Domain.Entities
{
    public class UserSubscription : Base
    {
        public Guid UserSubscriptionId { get; set; }

        public Guid UserId { get; set; }
        public Guid PackageId { get; set; }

        // Thời gian có hiệu lực của gói được mua
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        // Trạng thái (đã hết hạn, còn hạn, v.v...)
        public bool IsActive { get; set; } = true;

        public virtual User? User { get; set; }
        public virtual SubscriptionPackage? SubscriptionPackage { get; set; }
    }
}

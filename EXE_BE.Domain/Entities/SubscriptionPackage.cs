
using System.Collections.Generic;

namespace EXE_BE.Domain.Entities
{
    public class SubscriptionPackage : Base
    {
        public Guid PackageId { get; set; }
        public string Name { get; set; } = string.Empty; // Ví dụ: "Gói 1: Mở khóa vật phẩm", "Gói 2: Không quảng cáo"
        public string Description { get; set; } = string.Empty;

        public decimal Price { get; set; } = 0; // Giá tiền thật để mua gói
        public int DurationDays { get; set; } // Thời hạn của gói (ví dụ 30 ngày)

        // Các đặc quyền của gói
        public bool AllowsPremiumCosmetics { get; set; } = false; // Cho phép mua/mở khóa vật phẩm, đồ trang trí cao cấp (Gói 1 & Gói 2)
        public bool RemovesAds { get; set; } = false; // Không hiện video quảng cáo (Gói 2)

        public virtual ICollection<UserSubscription>? UserSubscriptions { get; set; }
    }
}

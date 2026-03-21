
using System.Collections.Generic;

namespace EXE_BE.Domain.Entities
{
    public class GameItem : Base
    {
        public Guid ItemId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        // Price to buy from the focus points/coins earned
        public int PriceCoins { get; set; }

        // Vật phẩm mở khóa theo cấp độ người dùng
        public int UnlockLevel { get; set; } = 1;

        // Example: "VirtualPet", "Theme", "Badge", "MusicTrack"
        public string ItemType { get; set; } = string.Empty;

        // Icon or Image URL for the item
        public string ImageUrl { get; set; } = string.Empty;

        // Chỉ định vật phẩm/đồ trang trí này yêu cầu gói đăng ký mới được phép mở khóa
        public bool IsPremiumOnly { get; set; } = false;

        // Thêm trường cho loại thú cưng (ví dụ: "Cat", "Fish", "Dog")
        public string? AnimalType { get; set; }

        // Đối với đồ ăn, xác định loại thú cưng nào có thể ăn (ví dụ: "Cat")
        // Chỉ những thú cưng có AnimalType khớp mới có thể ăn đồ ăn này.
        public string? TargetAnimalType { get; set; }

        public virtual ICollection<UserItem>? UserItems { get; set; }
        public virtual ICollection<UserPurchase>? Purchases { get; set; }
        public virtual ICollection<UserItemPosition>? ItemPositions { get; set; }
    }
}

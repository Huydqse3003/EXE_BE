namespace EXE_BE.Application.DTOs.Responses.Gameplay
{
    public class ShopItemResponse
    {
        public Guid ItemId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int PriceCoins { get; set; }
        public int UnlockLevel { get; set; }
        public string ItemType { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsPremiumOnly { get; set; }
        public string? AnimalType { get; set; }
        public string? TargetAnimalType { get; set; }
    }
}

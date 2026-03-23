namespace EXE_BE.Application.DTOs.Responses.Gameplay
{
    public class PurchaseItemResponse
    {
        public Guid PurchaseId { get; set; }
        public Guid UserId { get; set; }
        public Guid ItemId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public int CoinsSpent { get; set; }
        public int RemainingCoins { get; set; }
        public DateTime PurchaseDate { get; set; }
    }
}

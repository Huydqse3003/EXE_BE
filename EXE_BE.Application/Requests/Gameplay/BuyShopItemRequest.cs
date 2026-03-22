namespace EXE_BE.Application.DTOs.Requests.Gameplay
{
    public class BuyShopItemRequest
    {
        public Guid UserId { get; set; }
        public Guid ItemId { get; set; }
    }
}

namespace EXE_BE.Application.DTOs.Responses.Friendship
{
    public class FriendshipResponse
    {
        public Guid FriendshipId { get; set; }
        public Guid UserId { get; set; }
        public Guid FriendId { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}

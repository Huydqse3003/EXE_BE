namespace EXE_BE.Application.DTOs.Requests.Friendship
{
    public class CreateFriendshipRequest
    {
        public Guid UserId { get; set; }
        public Guid FriendId { get; set; }
    }
}

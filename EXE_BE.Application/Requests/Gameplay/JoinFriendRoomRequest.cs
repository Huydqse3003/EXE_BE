namespace EXE_BE.Application.DTOs.Requests.Gameplay
{
    public class JoinFriendRoomRequest
    {
        public Guid UserId { get; set; }
        public Guid FriendUserId { get; set; }
    }
}

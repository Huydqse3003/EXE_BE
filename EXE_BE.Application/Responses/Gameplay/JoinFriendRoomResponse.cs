namespace EXE_BE.Application.DTOs.Responses.Gameplay
{
    public class JoinFriendRoomResponse
    {
        public Guid CharacterId { get; set; }
        public Guid UserId { get; set; }
        public Guid FriendUserId { get; set; }
        public Guid VirtualRoomId { get; set; }
        public string ActionState { get; set; } = string.Empty;
        public string Direction { get; set; } = string.Empty;
    }
}

namespace EXE_BE.Application.DTOs.Requests.Gameplay
{
    public class JoinStudyRoomRequest
    {
        public Guid RoomId { get; set; }
        public Guid UserId { get; set; }
        public string? Passcode { get; set; }
    }
}

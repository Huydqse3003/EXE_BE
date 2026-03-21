namespace EXE_BE.Application.DTOs.Requests.Gameplay
{
    public class LeaveStudyRoomRequest
    {
        public Guid RoomId { get; set; }
        public Guid UserId { get; set; }
    }
}

namespace EXE_BE.Application.DTOs.Responses.Gameplay
{
    public class StudyRoomFlowResponse
    {
        public Guid RoomId { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsPrivate { get; set; }
        public int MaxCapacity { get; set; }
        public int ActiveMembers { get; set; }
        public bool IsDeleted { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}

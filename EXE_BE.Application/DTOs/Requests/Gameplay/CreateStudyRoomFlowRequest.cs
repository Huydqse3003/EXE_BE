namespace EXE_BE.Application.DTOs.Requests.Gameplay
{
    public class CreateStudyRoomFlowRequest
    {
        public Guid CreatedByUserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int MaxCapacity { get; set; } = 10;
        public bool IsPrivate { get; set; }
        public string? Passcode { get; set; }
    }
}

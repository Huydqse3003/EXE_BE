namespace EXE_BE.Application.DTOs.Requests.Gameplay
{
    public class DecorateVirtualRoomRequest
    {
        public Guid UserId { get; set; }
        public Guid VirtualRoomId { get; set; }
        public Guid ItemId { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public string? ActionState { get; set; }
        public string? MovementPattern { get; set; }
        public string? Direction { get; set; }
    }
}

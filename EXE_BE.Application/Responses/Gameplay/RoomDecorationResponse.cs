namespace EXE_BE.Application.DTOs.Responses.Gameplay
{
    public class RoomDecorationResponse
    {
        public Guid PositionId { get; set; }
        public Guid UserId { get; set; }
        public Guid ItemId { get; set; }
        public Guid VirtualRoomId { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public string ActionState { get; set; } = string.Empty;
        public string MovementPattern { get; set; } = string.Empty;
        public string Direction { get; set; } = string.Empty;
    }
}

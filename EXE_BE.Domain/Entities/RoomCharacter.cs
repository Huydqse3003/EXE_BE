using System;

namespace EXE_BE.Domain.Entities
{
    public class RoomCharacter : Base
    {
        public Guid CharacterId { get; set; }
        public Guid VirtualRoomId { get; set; }
        public Guid UserId { get; set; }
        
        // Tọa độ của người trong phòng để di chuyển
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        
        // Trạng thái của nhân vật (ví dụ: "Idle", "Walking", "Studying", "Sitting")
        public string ActionState { get; set; } = "Idle";
        
        // Hướng nhìn của nhân vật (ví dụ: 0-360 độ hoặc "Up", "Down", "Left", "Right")
        public string Direction { get; set; } = "Down";

        public virtual VirtualRoom? VirtualRoom { get; set; }
        public virtual User? User { get; set; }
    }
}

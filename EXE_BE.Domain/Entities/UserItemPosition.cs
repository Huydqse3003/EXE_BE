namespace EXE_BE.Domain.Entities
{
    public class UserItemPosition : Base
    {
        public Guid PositionId { get; set; }
        public Guid UserId { get; set; }
        public Guid ItemId { get; set; }

        // Thêm liên kết tới phòng ảo mà vật thể (đồ vật/con vật) đang được bố trí
        public Guid? VirtualRoomId { get; set; }

        // Tọa độ của vật phẩm/con vật trong phòng/ứng dụng
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; } // Nếu là không gian 3D, nếu 2D thì có thể sử dụng cho thứ tự lớp (z-index)

        public string? RoomMode { get; set; } // Nếu có nhiều phòng hoặc chế độ khác nhau thì dùng phân biệt

        // Nếu item là thú cưng (Pet), có thể có trạng thái hoặc hành động (Walking, Sleeping, Playing)
        public string ActionState { get; set; } = "Static";

        // Hành vi di chuyển do người dùng cài đặt ("Stationary" - Nằm im, "Wandering" - Đi dạo xung quanh)
        public string MovementPattern { get; set; } = "Stationary";

        // Hướng nhìn của thú cưng (nếu có thể di chuyển)
        public string Direction { get; set; } = "Down";

        // Mức độ no của thú cưng (dành cho GameItem là Pet) (ví dụ: 0 là đói, 100 là no)
        public int HungerLevel { get; set; } = 100;

        // Thời gian được cho ăn lần cuối cùng
        public DateTime? LastFedTime { get; set; }

        public virtual User? User { get; set; }
        public virtual GameItem? Item { get; set; }
        public virtual VirtualRoom? VirtualRoom { get; set; }
    }
}
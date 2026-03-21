using System;
using System.Collections.Generic;

namespace EXE_BE.Domain.Entities
{
    public class VirtualRoom : Base
    {
        public Guid VirtualRoomId { get; set; }
        public Guid UserId { get; set; } // Chủ sở hữu phòng (nếu là phòng cá nhân)
        
        public string Name { get; set; } = "My Virtual Room";
        public string Description { get; set; } = string.Empty;
        
        // Cài đặt chung của phòng như nền, tường, sàn
        public string BackgroundImageUrl { get; set; } = string.Empty;

        public virtual User? User { get; set; }
        
        // Các vật phẩm (đồ trang trí, thú cưng) được đặt trong phòng
        public virtual ICollection<UserItemPosition>? ItemPositions { get; set; }
        
        // Những người đang có mặt trong phòng
        public virtual ICollection<RoomCharacter>? Characters { get; set; }
    }
}

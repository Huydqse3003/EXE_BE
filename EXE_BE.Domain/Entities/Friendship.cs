using System;

namespace EXE_BE.Domain.Entities
{
    public class Friendship : Base
    {
        public Guid FriendshipId { get; set; }
        
        // Người gửi lời mời
        public Guid UserId { get; set; }
        
        // Người nhận lời mời
        public Guid FriendId { get; set; }
        
        // Trạng thái tình bạn (ví dụ: "Pending", "Accepted", "Blocked")
        public string Status { get; set; } = "Pending";
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual User? User { get; set; }
        public virtual User? Friend { get; set; }
    }
}

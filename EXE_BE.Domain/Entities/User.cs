namespace EXE_BE.Domain.Entities
{
    public class User : Base
    {
        public Guid UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int Age { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public Guid? CurrentFocusMusicId { get; set; }
        public string PasswordHash { get; set; } = string.Empty;

        // Gamification stats
        public int Level { get; set; } = 1;
        public int ExperiencePoints { get; set; } = 0;
        public int Coins { get; set; } = 0;

        // Navigation properties
        public virtual ICollection<FocusSession>? FocusSessions { get; set; }
        public virtual ICollection<Topic>? Topics { get; set; }
        public virtual ICollection<UserItem>? UserItems { get; set; }
        public virtual ICollection<UserSubscription>? Subscriptions { get; set; }
        public virtual ICollection<UserPurchase>? Purchases { get; set; }
        public virtual ICollection<UserCoinTransaction>? CoinTransactions { get; set; }
        public virtual ICollection<UserItemPosition>? ItemPositions { get; set; }

        // Study Room properties
        public virtual ICollection<StudyRoom>? CreatedRooms { get; set; }
        public virtual ICollection<StudyRoomMember>? JoinedRooms { get; set; }

        // Virtual Room properties
        public virtual ICollection<VirtualRoom>? VirtualRooms { get; set; }
        public virtual ICollection<RoomCharacter>? RoomCharacters { get; set; }

        // Friendship properties
        public virtual ICollection<Friendship>? SentFriendRequests { get; set; }
        public virtual ICollection<Friendship>? ReceivedFriendRequests { get; set; }
        public virtual ICollection<UserHabit>? Habits { get; set; }
        public virtual ICollection<FocusMusic>? FocusMusics { get; set; }
        public virtual FocusMusic? CurrentFocusMusic { get; set; }
    }
}

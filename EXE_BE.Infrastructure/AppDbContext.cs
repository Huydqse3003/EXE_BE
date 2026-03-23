using Microsoft.EntityFrameworkCore;
using EXE_BE.Domain.Entities;

namespace EXE_BE.Infrastructure
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<FocusSession> FocusSessions { get; set; }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<GameItem> GameItems { get; set; }
        public DbSet<UserItem> UserItems { get; set; }
        public DbSet<SubscriptionPackage> SubscriptionPackages { get; set; }
        public DbSet<UserSubscription> UserSubscriptions { get; set; }
        public DbSet<PaymentTransaction> PaymentTransactions { get; set; }
        public DbSet<UserPurchase> UserPurchases { get; set; }
        public DbSet<UserCoinTransaction> UserCoinTransactions { get; set; }
        public DbSet<StudyRoom> StudyRooms { get; set; }
        public DbSet<StudyRoomMember> StudyRoomMembers { get; set; }
        public DbSet<UserItemPosition> UserItemPositions { get; set; }
        public DbSet<VirtualRoom> VirtualRooms { get; set; }
        public DbSet<RoomCharacter> RoomCharacters { get; set; }
        public DbSet<Friendship> Friendships { get; set; }
        public DbSet<UserHabit> UserHabits { get; set; }
        public DbSet<FocusMusic> FocusMusics { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình khoá chính (Primary Keys) vì tên không theo chuẩn mặc định của EF (Id hoặc ClassNameId)
            modelBuilder.Entity<FocusSession>().HasKey(fs => fs.SessionId);
            modelBuilder.Entity<Topic>().HasKey(t => t.TopicId);
            modelBuilder.Entity<GameItem>().HasKey(g => g.ItemId);
            modelBuilder.Entity<SubscriptionPackage>().HasKey(sp => sp.PackageId);
            modelBuilder.Entity<PaymentTransaction>().HasKey(pt => pt.PaymentTransactionId);
            modelBuilder.Entity<UserPurchase>().HasKey(up => up.PurchaseId);
            modelBuilder.Entity<UserCoinTransaction>().HasKey(uct => uct.TransactionId);
            modelBuilder.Entity<StudyRoom>().HasKey(sr => sr.RoomId);
            modelBuilder.Entity<UserItemPosition>().HasKey(uip => uip.PositionId);
            modelBuilder.Entity<VirtualRoom>().HasKey(vr => vr.VirtualRoomId);
            modelBuilder.Entity<RoomCharacter>().HasKey(rc => rc.CharacterId);
            modelBuilder.Entity<Friendship>().HasKey(f => f.FriendshipId);
            modelBuilder.Entity<UserHabit>().HasKey(h => h.HabitId);
            modelBuilder.Entity<FocusMusic>().HasKey(fm => fm.MusicId);

            // Cấu hình khoá chính kết hợp (Composite Keys) cho các bảng trung gian
            modelBuilder.Entity<UserItem>()
                .HasKey(ui => new { ui.UserId, ui.ItemId });

            modelBuilder.Entity<StudyRoomMember>()
                .HasKey(sm => new { sm.RoomId, sm.UserId });

            // Các cấu hình Fluent API khác nếu cần

            // Ví dụ: StudyRoom và Room CreatedBy
            modelBuilder.Entity<StudyRoom>()
                .HasOne(sr => sr.CreatedByUser)
                .WithMany(u => u.CreatedRooms)
                .HasForeignKey(sr => sr.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Quan hệ User - JoinedRooms thông qua StudyRoomMember
            modelBuilder.Entity<StudyRoomMember>()
                .HasOne(sm => sm.User)
                .WithMany(u => u.JoinedRooms)
                .HasForeignKey(sm => sm.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<StudyRoomMember>()
                .HasOne(sm => sm.Room)
                .WithMany(sr => sr.Members)
                .HasForeignKey(sm => sm.RoomId)
                .OnDelete(DeleteBehavior.Cascade);

            // Quan hệ User - VirtualRooms
            modelBuilder.Entity<VirtualRoom>()
                .HasOne(vr => vr.User)
                .WithMany(u => u.VirtualRooms)
                .HasForeignKey(vr => vr.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Không cascade delete user

            // Cấu hình cho RoomCharacter
            modelBuilder.Entity<RoomCharacter>()
                .HasOne(rc => rc.User)
                .WithMany(u => u.RoomCharacters)
                .HasForeignKey(rc => rc.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RoomCharacter>()
                .HasOne(rc => rc.VirtualRoom)
                .WithMany(vr => vr.Characters)
                .HasForeignKey(rc => rc.VirtualRoomId)
                .OnDelete(DeleteBehavior.Cascade);

            // Mối quan hệ VirtualRoom - UserItemPosition
            modelBuilder.Entity<UserItemPosition>()
                .HasOne(uip => uip.VirtualRoom)
                .WithMany(vr => vr.ItemPositions)
                .HasForeignKey(uip => uip.VirtualRoomId)
                .OnDelete(DeleteBehavior.Cascade);

            // Quan hệ Friendship (Bạn bè) 
            modelBuilder.Entity<Friendship>()
                .HasOne(f => f.User)
                .WithMany(u => u.SentFriendRequests)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Friendship>()
                .HasOne(f => f.Friend)
                .WithMany(u => u.ReceivedFriendRequests)
                .HasForeignKey(f => f.FriendId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserHabit>()
                .HasOne(h => h.User)
                .WithMany(u => u.Habits)
                .HasForeignKey(h => h.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FocusMusic>()
                .HasOne(fm => fm.User)
                .WithMany(u => u.FocusMusics)
                .HasForeignKey(fm => fm.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasOne(u => u.CurrentFocusMusic)
                .WithMany()
                .HasForeignKey(u => u.CurrentFocusMusicId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}

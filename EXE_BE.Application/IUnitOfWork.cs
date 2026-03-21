using EXE_BE.Application.IRepositories;

using System.Threading.Tasks;

namespace EXE_BE.Application
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        IFocusSessionRepository FocusSessions { get; }
        ITopicRepository Topics { get; }
        IGameItemRepository GameItems { get; }
        IUserItemRepository UserItems { get; }
        ISubscriptionPackageRepository SubscriptionPackages { get; }
        IUserSubscriptionRepository UserSubscriptions { get; }
        IUserPurchaseRepository UserPurchases { get; }
        IUserCoinTransactionRepository UserCoinTransactions { get; }
        IStudyRoomRepository StudyRooms { get; }
        IStudyRoomMemberRepository StudyRoomMembers { get; }
        IUserItemPositionRepository UserItemPositions { get; }
        IVirtualRoomRepository VirtualRooms { get; }
        IRoomCharacterRepository RoomCharacters { get; }
        IFriendshipRepository Friendships { get; }
        IUserHabitRepository UserHabits { get; }
        IFocusMusicRepository FocusMusics { get; }

        Task<int> SaveChangesAsync();
        Task<T> ExecuteScalarAsync<T>(string sql);
        Task ExecuteRawSqlAsync(string sql);
    }
}

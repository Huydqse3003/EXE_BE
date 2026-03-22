using System.Data;
using Microsoft.EntityFrameworkCore;
using EXE_BE.Application;
using EXE_BE.Application.IRepositories;
using EXE_BE.Infrastructure.Repositories;

namespace EXE_BE.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public IUserRepository Users { get; }
        public IFocusSessionRepository FocusSessions { get; }
        public ITopicRepository Topics { get; }
        public IGameItemRepository GameItems { get; }
        public IUserItemRepository UserItems { get; }
        public ISubscriptionPackageRepository SubscriptionPackages { get; }
        public IUserSubscriptionRepository UserSubscriptions { get; }
        public IPaymentTransactionRepository PaymentTransactions { get; }
        public IUserPurchaseRepository UserPurchases { get; }
        public IUserCoinTransactionRepository UserCoinTransactions { get; }
        public IStudyRoomRepository StudyRooms { get; }
        public IStudyRoomMemberRepository StudyRoomMembers { get; }
        public IUserItemPositionRepository UserItemPositions { get; }
        public IVirtualRoomRepository VirtualRooms { get; }
        public IRoomCharacterRepository RoomCharacters { get; }
        public IFriendshipRepository Friendships { get; }
        public IUserHabitRepository UserHabits { get; }
        public IFocusMusicRepository FocusMusics { get; }

        public UnitOfWork(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));

            Users = new UserRepository(context);
            FocusSessions = new FocusSessionRepository(context);
            Topics = new TopicRepository(context);
            GameItems = new GameItemRepository(context);
            UserItems = new UserItemRepository(context);
            SubscriptionPackages = new SubscriptionPackageRepository(context);
            UserSubscriptions = new UserSubscriptionRepository(context);
            PaymentTransactions = new PaymentTransactionRepository(context);
            UserPurchases = new UserPurchaseRepository(context);
            UserCoinTransactions = new UserCoinTransactionRepository(context);
            StudyRooms = new StudyRoomRepository(context);
            StudyRoomMembers = new StudyRoomMemberRepository(context);
            UserItemPositions = new UserItemPositionRepository(context);
            VirtualRooms = new VirtualRoomRepository(context);
            RoomCharacters = new RoomCharacterRepository(context);
            Friendships = new FriendshipRepository(context);
            UserHabits = new UserHabitRepository(context);
            FocusMusics = new FocusMusicRepository(context);
        }

        public async Task<int> SaveChangesAsync()
        {
            try
            {
                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<T> ExecuteScalarAsync<T>(string sql)
        {
            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                try
                {
                    if (command.Connection!.State != ConnectionState.Open)
                    {
                        await command.Connection.OpenAsync();
                    }

                    command.CommandText = sql;
                    var result = await command.ExecuteScalarAsync();
                    return (T)Convert.ChangeType(result, typeof(T))!;
                }
                finally
                {
                    if (command.Connection!.State == ConnectionState.Open)
                    {
                        await command.Connection.CloseAsync();
                    }
                }
            }
        }

        public async Task ExecuteRawSqlAsync(string sql)
        {
            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                try
                {
                    if (command.Connection!.State != ConnectionState.Open)
                    {
                        await command.Connection.OpenAsync();
                    }

                    command.CommandText = sql;
                    await command.ExecuteNonQueryAsync();
                }
                finally
                {
                    if (command.Connection!.State == ConnectionState.Open)
                    {
                        await command.Connection.CloseAsync();
                    }
                }
            }
        }

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}

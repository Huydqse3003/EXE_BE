using EXE_BE.Application.IRepositories;
using EXE_BE.Domain.Entities;


namespace EXE_BE.Infrastructure.Repositories
{
    public class UserCoinTransactionRepository : GenericRepository<UserCoinTransaction>, IUserCoinTransactionRepository
    {
        public UserCoinTransactionRepository(AppDbContext context) : base(context)
        {
        }
    }
}

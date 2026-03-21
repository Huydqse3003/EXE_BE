using EXE_BE.Application.IRepositories;
using EXE_BE.Domain.Entities;


namespace EXE_BE.Infrastructure.Repositories
{
    public class UserPurchaseRepository : GenericRepository<UserPurchase>, IUserPurchaseRepository
    {
        public UserPurchaseRepository(AppDbContext context) : base(context)
        {
        }
    }
}

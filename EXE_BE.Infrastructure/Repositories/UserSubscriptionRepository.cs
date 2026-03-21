using EXE_BE.Application.IRepositories;
using EXE_BE.Domain.Entities;


namespace EXE_BE.Infrastructure.Repositories
{
    public class UserSubscriptionRepository : GenericRepository<UserSubscription>, IUserSubscriptionRepository
    {
        public UserSubscriptionRepository(AppDbContext context) : base(context)
        {
        }
    }
}

using EXE_BE.Application.IRepositories;
using EXE_BE.Domain.Entities;


namespace EXE_BE.Infrastructure.Repositories
{
    public class SubscriptionPackageRepository : GenericRepository<SubscriptionPackage>, ISubscriptionPackageRepository
    {
        public SubscriptionPackageRepository(AppDbContext context) : base(context)
        {
        }
    }
}

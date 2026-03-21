using EXE_BE.Application.IRepositories;
using EXE_BE.Domain.Entities;

namespace EXE_BE.Infrastructure.Repositories
{
    public class UserItemPositionRepository : GenericRepository<UserItemPosition>, IUserItemPositionRepository
    {
        public UserItemPositionRepository(AppDbContext context) : base(context)
        {
        }
    }
}

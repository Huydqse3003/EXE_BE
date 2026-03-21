using EXE_BE.Application.IRepositories;
using EXE_BE.Domain.Entities;


namespace EXE_BE.Infrastructure.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context)
        {
        }
    }
}

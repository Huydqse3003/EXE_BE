using EXE_BE.Application.IRepositories;
using EXE_BE.Domain.Entities;

namespace EXE_BE.Infrastructure.Repositories
{
    public class UserHabitRepository : GenericRepository<UserHabit>, IUserHabitRepository
    {
        public UserHabitRepository(AppDbContext context) : base(context)
        {
        }
    }
}

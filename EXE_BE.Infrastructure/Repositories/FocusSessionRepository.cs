using EXE_BE.Application.IRepositories;
using EXE_BE.Domain.Entities;


namespace EXE_BE.Infrastructure.Repositories
{
    public class FocusSessionRepository : GenericRepository<FocusSession>, IFocusSessionRepository
    {
        public FocusSessionRepository(AppDbContext context) : base(context)
        {
        }
    }
}

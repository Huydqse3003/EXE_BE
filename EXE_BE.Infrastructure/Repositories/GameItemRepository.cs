using EXE_BE.Application.IRepositories;
using EXE_BE.Domain.Entities;

namespace EXE_BE.Infrastructure.Repositories
{
    public class GameItemRepository : GenericRepository<GameItem>, IGameItemRepository
    {
        public GameItemRepository(AppDbContext context) : base(context)
        {
        }
    }
}

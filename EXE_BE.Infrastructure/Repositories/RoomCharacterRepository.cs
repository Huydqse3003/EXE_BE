using EXE_BE.Application.IRepositories;
using EXE_BE.Domain.Entities;

namespace EXE_BE.Infrastructure.Repositories
{
    public class RoomCharacterRepository : GenericRepository<RoomCharacter>, IRoomCharacterRepository
    {
        public RoomCharacterRepository(AppDbContext context) : base(context)
        {
        }
    }
}

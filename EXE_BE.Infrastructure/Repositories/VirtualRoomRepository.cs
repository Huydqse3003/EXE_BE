using EXE_BE.Application.IRepositories;
using EXE_BE.Domain.Entities;

namespace EXE_BE.Infrastructure.Repositories
{
    public class VirtualRoomRepository : GenericRepository<VirtualRoom>, IVirtualRoomRepository
    {
        public VirtualRoomRepository(AppDbContext context) : base(context)
        {
        }
    }
}

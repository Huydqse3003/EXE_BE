using EXE_BE.Application.IRepositories;
using EXE_BE.Domain.Entities;


namespace EXE_BE.Infrastructure.Repositories
{
    public class StudyRoomRepository : GenericRepository<StudyRoom>, IStudyRoomRepository
    {
        public StudyRoomRepository(AppDbContext context) : base(context)
        {
        }
    }
}

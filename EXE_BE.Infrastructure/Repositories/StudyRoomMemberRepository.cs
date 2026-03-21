using EXE_BE.Application.IRepositories;
using EXE_BE.Domain.Entities;


namespace EXE_BE.Infrastructure.Repositories
{
    public class StudyRoomMemberRepository : GenericRepository<StudyRoomMember>, IStudyRoomMemberRepository
    {
        public StudyRoomMemberRepository(AppDbContext context) : base(context)
        {
        }
    }
}

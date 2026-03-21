using EXE_BE.Application.IRepositories;
using EXE_BE.Domain.Entities;


namespace EXE_BE.Infrastructure.Repositories
{
    public class TopicRepository : GenericRepository<Topic>, ITopicRepository
    {
        public TopicRepository(AppDbContext context) : base(context)
        {
        }
    }
}

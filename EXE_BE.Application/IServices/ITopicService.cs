using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EXE_BE.Application.DTOs.Requests.Topic;
using EXE_BE.Application.DTOs.Responses.Topic;

namespace EXE_BE.Application.IServices
{
    public interface ITopicService
    {
        Task<IEnumerable<TopicResponse>> GetAllAsync();
        Task<TopicResponse?> GetByIdAsync(Guid id);
        Task AddAsync(CreateTopicRequest request);
        Task UpdateAsync(Guid id, UpdateTopicRequest request);
        Task DeleteAsync(Guid id);
    }
}

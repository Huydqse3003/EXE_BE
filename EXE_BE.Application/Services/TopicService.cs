using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EXE_BE.Application.DTOs.Requests.Topic;
using EXE_BE.Application.DTOs.Responses.Topic;
using EXE_BE.Application.IServices;
using EXE_BE.Domain.Entities;

namespace EXE_BE.Application.Services
{
    public class TopicService : ITopicService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TopicService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<TopicResponse>> GetAllAsync()
        {
            var entities = await _unitOfWork.Topics.GetAllAsync();
            // TODO: Map from Entity to Response DTO
            return entities.Select(e => new TopicResponse()); 
        }

        public async Task<TopicResponse?> GetByIdAsync(Guid id)
        {
            var entity = await _unitOfWork.Topics.GetByIdAsync(id);
            if (entity == null) return null;
            
            // TODO: Map from Entity to Response DTO
            return new TopicResponse();
        }

        public async Task AddAsync(CreateTopicRequest request)
        {
            // TODO: Map from Request DTO to Entity
            var entity = new Topic();
            
            await _unitOfWork.Topics.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateAsync(Guid id, UpdateTopicRequest request)
        {
            var entity = await _unitOfWork.Topics.GetByIdAsync(id);
            if (entity != null)
            {
                // TODO: Apply updates from Request DTO to Entity

                _unitOfWork.Topics.Update(entity);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _unitOfWork.Topics.GetByIdAsync(id);
            if (entity != null)
            {
                _unitOfWork.Topics.Remove(entity);
                await _unitOfWork.SaveChangesAsync();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EXE_BE.Application.DTOs.Requests.UserSubscription;
using EXE_BE.Application.DTOs.Responses.UserSubscription;
using EXE_BE.Application.IServices;
using EXE_BE.Domain.Entities;

namespace EXE_BE.Application.Services
{
    public class UserSubscriptionService : IUserSubscriptionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserSubscriptionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<UserSubscriptionResponse>> GetAllAsync()
        {
            var entities = await _unitOfWork.UserSubscriptions.GetAllAsync();
            // TODO: Map from Entity to Response DTO
            return entities.Select(e => new UserSubscriptionResponse()); 
        }

        public async Task<UserSubscriptionResponse?> GetByIdAsync(Guid id)
        {
            var entity = await _unitOfWork.UserSubscriptions.GetByIdAsync(id);
            if (entity == null) return null;
            
            // TODO: Map from Entity to Response DTO
            return new UserSubscriptionResponse();
        }

        public async Task AddAsync(CreateUserSubscriptionRequest request)
        {
            // TODO: Map from Request DTO to Entity
            var entity = new UserSubscription();
            
            await _unitOfWork.UserSubscriptions.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateAsync(Guid id, UpdateUserSubscriptionRequest request)
        {
            var entity = await _unitOfWork.UserSubscriptions.GetByIdAsync(id);
            if (entity != null)
            {
                // TODO: Apply updates from Request DTO to Entity

                _unitOfWork.UserSubscriptions.Update(entity);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _unitOfWork.UserSubscriptions.GetByIdAsync(id);
            if (entity != null)
            {
                _unitOfWork.UserSubscriptions.Remove(entity);
                await _unitOfWork.SaveChangesAsync();
            }
        }
    }
}

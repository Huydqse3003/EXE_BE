using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EXE_BE.Application.DTOs.Requests.UserPurchase;
using EXE_BE.Application.DTOs.Responses.UserPurchase;
using EXE_BE.Application.IServices;
using EXE_BE.Domain.Entities;

namespace EXE_BE.Application.Services
{
    public class UserPurchaseService : IUserPurchaseService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserPurchaseService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<UserPurchaseResponse>> GetAllAsync()
        {
            var entities = await _unitOfWork.UserPurchases.GetAllAsync();
            // TODO: Map from Entity to Response DTO
            return entities.Select(e => new UserPurchaseResponse()); 
        }

        public async Task<UserPurchaseResponse?> GetByIdAsync(Guid id)
        {
            var entity = await _unitOfWork.UserPurchases.GetByIdAsync(id);
            if (entity == null) return null;
            
            // TODO: Map from Entity to Response DTO
            return new UserPurchaseResponse();
        }

        public async Task AddAsync(CreateUserPurchaseRequest request)
        {
            // TODO: Map from Request DTO to Entity
            var entity = new UserPurchase();
            
            await _unitOfWork.UserPurchases.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateAsync(Guid id, UpdateUserPurchaseRequest request)
        {
            var entity = await _unitOfWork.UserPurchases.GetByIdAsync(id);
            if (entity != null)
            {
                // TODO: Apply updates from Request DTO to Entity

                _unitOfWork.UserPurchases.Update(entity);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _unitOfWork.UserPurchases.GetByIdAsync(id);
            if (entity != null)
            {
                _unitOfWork.UserPurchases.Remove(entity);
                await _unitOfWork.SaveChangesAsync();
            }
        }
    }
}

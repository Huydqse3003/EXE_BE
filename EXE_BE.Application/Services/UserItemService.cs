using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EXE_BE.Application.DTOs.Requests.UserItem;
using EXE_BE.Application.DTOs.Responses.UserItem;
using EXE_BE.Application.IServices;
using EXE_BE.Domain.Entities;

namespace EXE_BE.Application.Services
{
    public class UserItemService : IUserItemService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserItemService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<UserItemResponse>> GetAllAsync()
        {
            var entities = await _unitOfWork.UserItems.GetAllAsync();
            // TODO: Map from Entity to Response DTO
            return entities.Select(e => new UserItemResponse()); 
        }

        public async Task<UserItemResponse?> GetByIdAsync(Guid id)
        {
            var entity = await _unitOfWork.UserItems.GetByIdAsync(id);
            if (entity == null) return null;
            
            // TODO: Map from Entity to Response DTO
            return new UserItemResponse();
        }

        public async Task AddAsync(CreateUserItemRequest request)
        {
            // TODO: Map from Request DTO to Entity
            var entity = new UserItem();
            
            await _unitOfWork.UserItems.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateAsync(Guid id, UpdateUserItemRequest request)
        {
            var entity = await _unitOfWork.UserItems.GetByIdAsync(id);
            if (entity != null)
            {
                // TODO: Apply updates from Request DTO to Entity

                _unitOfWork.UserItems.Update(entity);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _unitOfWork.UserItems.GetByIdAsync(id);
            if (entity != null)
            {
                _unitOfWork.UserItems.Remove(entity);
                await _unitOfWork.SaveChangesAsync();
            }
        }
    }
}

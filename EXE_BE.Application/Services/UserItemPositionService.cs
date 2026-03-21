using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EXE_BE.Application.DTOs.Requests.UserItemPosition;
using EXE_BE.Application.DTOs.Responses.UserItemPosition;
using EXE_BE.Application.IServices;
using EXE_BE.Domain.Entities;

namespace EXE_BE.Application.Services
{
    public class UserItemPositionService : IUserItemPositionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserItemPositionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<UserItemPositionResponse>> GetAllAsync()
        {
            var entities = await _unitOfWork.UserItemPositions.GetAllAsync();
            return entities.Select(e => new UserItemPositionResponse()); 
        }

        public async Task<UserItemPositionResponse?> GetByIdAsync(Guid id)
        {
            var entity = await _unitOfWork.UserItemPositions.GetByIdAsync(id);
            if (entity == null) return null;
            return new UserItemPositionResponse();
        }

        public async Task AddAsync(CreateUserItemPositionRequest request)
        {
            var entity = new UserItemPosition();
            await _unitOfWork.UserItemPositions.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateAsync(Guid id, UpdateUserItemPositionRequest request)
        {
            var entity = await _unitOfWork.UserItemPositions.GetByIdAsync(id);
            if (entity != null)
            {
                _unitOfWork.UserItemPositions.Update(entity);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _unitOfWork.UserItemPositions.GetByIdAsync(id);
            if (entity != null)
            {
                _unitOfWork.UserItemPositions.Remove(entity);
                await _unitOfWork.SaveChangesAsync();
            }
        }
    }
}

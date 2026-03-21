using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EXE_BE.Application.DTOs.Requests.GameItem;
using EXE_BE.Application.DTOs.Responses.GameItem;
using EXE_BE.Application.IServices;
using EXE_BE.Domain.Entities;

namespace EXE_BE.Application.Services
{
    public class GameItemService : IGameItemService
    {
        private readonly IUnitOfWork _unitOfWork;

        public GameItemService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<GameItemResponse>> GetAllAsync()
        {
            var entities = await _unitOfWork.GameItems.GetAllAsync();
            // TODO: Map from Entity to Response DTO
            return entities.Select(e => new GameItemResponse()); 
        }

        public async Task<GameItemResponse?> GetByIdAsync(Guid id)
        {
            var entity = await _unitOfWork.GameItems.GetByIdAsync(id);
            if (entity == null) return null;
            
            // TODO: Map from Entity to Response DTO
            return new GameItemResponse();
        }

        public async Task AddAsync(CreateGameItemRequest request)
        {
            // TODO: Map from Request DTO to Entity
            var entity = new GameItem();
            
            await _unitOfWork.GameItems.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateAsync(Guid id, UpdateGameItemRequest request)
        {
            var entity = await _unitOfWork.GameItems.GetByIdAsync(id);
            if (entity != null)
            {
                // TODO: Apply updates from Request DTO to Entity

                _unitOfWork.GameItems.Update(entity);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _unitOfWork.GameItems.GetByIdAsync(id);
            if (entity != null)
            {
                _unitOfWork.GameItems.Remove(entity);
                await _unitOfWork.SaveChangesAsync();
            }
        }
    }
}

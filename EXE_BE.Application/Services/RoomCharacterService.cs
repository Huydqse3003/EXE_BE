using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EXE_BE.Application.DTOs.Requests.RoomCharacter;
using EXE_BE.Application.DTOs.Responses.RoomCharacter;
using EXE_BE.Application.IServices;
using EXE_BE.Domain.Entities;

namespace EXE_BE.Application.Services
{
    public class RoomCharacterService : IRoomCharacterService
    {
        private readonly IUnitOfWork _unitOfWork;

        public RoomCharacterService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<RoomCharacterResponse>> GetAllAsync()
        {
            var entities = await _unitOfWork.RoomCharacters.GetAllAsync();
            return entities.Select(e => new RoomCharacterResponse()); 
        }

        public async Task<RoomCharacterResponse?> GetByIdAsync(Guid id)
        {
            var entity = await _unitOfWork.RoomCharacters.GetByIdAsync(id);
            if (entity == null) return null;
            return new RoomCharacterResponse();
        }

        public async Task AddAsync(CreateRoomCharacterRequest request)
        {
            var entity = new RoomCharacter();
            await _unitOfWork.RoomCharacters.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateAsync(Guid id, UpdateRoomCharacterRequest request)
        {
            var entity = await _unitOfWork.RoomCharacters.GetByIdAsync(id);
            if (entity != null)
            {
                _unitOfWork.RoomCharacters.Update(entity);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _unitOfWork.RoomCharacters.GetByIdAsync(id);
            if (entity != null)
            {
                _unitOfWork.RoomCharacters.Remove(entity);
                await _unitOfWork.SaveChangesAsync();
            }
        }
    }
}

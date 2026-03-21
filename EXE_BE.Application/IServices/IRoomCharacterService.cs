using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EXE_BE.Application.DTOs.Requests.RoomCharacter;
using EXE_BE.Application.DTOs.Responses.RoomCharacter;

namespace EXE_BE.Application.IServices
{
    public interface IRoomCharacterService
    {
        Task<IEnumerable<RoomCharacterResponse>> GetAllAsync();
        Task<RoomCharacterResponse?> GetByIdAsync(Guid id);
        Task AddAsync(CreateRoomCharacterRequest request);
        Task UpdateAsync(Guid id, UpdateRoomCharacterRequest request);
        Task DeleteAsync(Guid id);
    }
}

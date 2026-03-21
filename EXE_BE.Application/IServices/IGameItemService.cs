using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EXE_BE.Application.DTOs.Requests.GameItem;
using EXE_BE.Application.DTOs.Responses.GameItem;

namespace EXE_BE.Application.IServices
{
    public interface IGameItemService
    {
        Task<IEnumerable<GameItemResponse>> GetAllAsync();
        Task<GameItemResponse?> GetByIdAsync(Guid id);
        Task AddAsync(CreateGameItemRequest request);
        Task UpdateAsync(Guid id, UpdateGameItemRequest request);
        Task DeleteAsync(Guid id);
    }
}

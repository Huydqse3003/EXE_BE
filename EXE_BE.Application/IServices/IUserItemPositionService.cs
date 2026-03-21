using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EXE_BE.Application.DTOs.Requests.UserItemPosition;
using EXE_BE.Application.DTOs.Responses.UserItemPosition;

namespace EXE_BE.Application.IServices
{
    public interface IUserItemPositionService
    {
        Task<IEnumerable<UserItemPositionResponse>> GetAllAsync();
        Task<UserItemPositionResponse?> GetByIdAsync(Guid id);
        Task AddAsync(CreateUserItemPositionRequest request);
        Task UpdateAsync(Guid id, UpdateUserItemPositionRequest request);
        Task DeleteAsync(Guid id);
    }
}

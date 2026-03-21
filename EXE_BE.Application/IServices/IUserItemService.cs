using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EXE_BE.Application.DTOs.Requests.UserItem;
using EXE_BE.Application.DTOs.Responses.UserItem;

namespace EXE_BE.Application.IServices
{
    public interface IUserItemService
    {
        Task<IEnumerable<UserItemResponse>> GetAllAsync();
        Task<UserItemResponse?> GetByIdAsync(Guid id);
        Task AddAsync(CreateUserItemRequest request);
        Task UpdateAsync(Guid id, UpdateUserItemRequest request);
        Task DeleteAsync(Guid id);
    }
}

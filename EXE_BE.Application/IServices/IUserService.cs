using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EXE_BE.Application.DTOs.Requests.User;
using EXE_BE.Application.DTOs.Responses.User;

namespace EXE_BE.Application.IServices
{
    public interface IUserService
    {
        Task<IEnumerable<UserResponse>> GetAllAsync();
        Task<UserResponse?> GetByIdAsync(Guid id);
        Task AddAsync(CreateUserRequest request);
        Task UpdateAsync(Guid id, UpdateUserRequest request);
        Task DeleteAsync(Guid id);
    }
}

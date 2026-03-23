using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EXE_BE.Application.DTOs.Responses;
using EXE_BE.Application.DTOs.Requests.Friendship;
using EXE_BE.Application.DTOs.Responses.Friendship;

namespace EXE_BE.Application.IServices
{
    public interface IFriendshipService
    {
        Task<ApiResponse> GetAllAsync();
        Task<ApiResponse> GetByIdAsync(Guid id);
        Task<ApiResponse> AddAsync(CreateFriendshipRequest request);
        Task<ApiResponse> UpdateAsync(Guid id, UpdateFriendshipRequest request);
        Task<ApiResponse> GetPendingByUserIdAsync(Guid userId);
        Task<ApiResponse> DeleteAsync(Guid id);
    }
}

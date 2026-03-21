using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EXE_BE.Application.DTOs.Requests.Friendship;
using EXE_BE.Application.DTOs.Responses.Friendship;

namespace EXE_BE.Application.IServices
{
    public interface IFriendshipService
    {
        Task<IEnumerable<FriendshipResponse>> GetAllAsync();
        Task<FriendshipResponse?> GetByIdAsync(Guid id);
        Task<FriendshipResponse> AddAsync(CreateFriendshipRequest request);
        Task<FriendshipResponse?> UpdateAsync(Guid id, UpdateFriendshipRequest request);
        Task<IEnumerable<FriendshipResponse>> GetPendingByUserIdAsync(Guid userId);
        Task DeleteAsync(Guid id);
    }
}

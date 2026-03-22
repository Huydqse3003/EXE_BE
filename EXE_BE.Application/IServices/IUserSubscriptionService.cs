using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EXE_BE.Application.DTOs.Requests.UserSubscription;
using EXE_BE.Application.DTOs.Responses;
using EXE_BE.Application.DTOs.Responses.UserSubscription;

namespace EXE_BE.Application.IServices
{
    public interface IUserSubscriptionService
    {
        Task<ApiResponse> GetAllAsync();
        Task<ApiResponse> GetByIdAsync(Guid id);
        Task<ApiResponse> AddAsync(CreateUserSubscriptionRequest request);
        Task<ApiResponse> UpdateAsync(Guid id, UpdateUserSubscriptionRequest request);
        Task<ApiResponse> DeleteAsync(Guid id);
    }
}

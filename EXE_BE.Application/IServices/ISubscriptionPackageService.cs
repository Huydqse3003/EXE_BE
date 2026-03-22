using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EXE_BE.Application.DTOs.Requests.SubscriptionPackage;
using EXE_BE.Application.DTOs.Responses;
using EXE_BE.Application.DTOs.Responses.SubscriptionPackage;

namespace EXE_BE.Application.IServices
{
    public interface ISubscriptionPackageService
    {
        Task<ApiResponse> GetAllAsync();
        Task<ApiResponse> GetByIdAsync(Guid id);
        Task<ApiResponse> AddAsync(CreateSubscriptionPackageRequest request);
        Task<ApiResponse> UpdateAsync(Guid id, UpdateSubscriptionPackageRequest request);
        Task<ApiResponse> DeleteAsync(Guid id);
    }
}

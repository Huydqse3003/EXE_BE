using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EXE_BE.Application.DTOs.Requests.SubscriptionPackage;
using EXE_BE.Application.DTOs.Responses.SubscriptionPackage;

namespace EXE_BE.Application.IServices
{
    public interface ISubscriptionPackageService
    {
        Task<IEnumerable<SubscriptionPackageResponse>> GetAllAsync();
        Task<SubscriptionPackageResponse?> GetByIdAsync(Guid id);
        Task AddAsync(CreateSubscriptionPackageRequest request);
        Task UpdateAsync(Guid id, UpdateSubscriptionPackageRequest request);
        Task DeleteAsync(Guid id);
    }
}

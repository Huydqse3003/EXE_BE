using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EXE_BE.Application.DTOs.Requests.UserSubscription;
using EXE_BE.Application.DTOs.Responses.UserSubscription;

namespace EXE_BE.Application.IServices
{
    public interface IUserSubscriptionService
    {
        Task<IEnumerable<UserSubscriptionResponse>> GetAllAsync();
        Task<UserSubscriptionResponse?> GetByIdAsync(Guid id);
        Task AddAsync(CreateUserSubscriptionRequest request);
        Task UpdateAsync(Guid id, UpdateUserSubscriptionRequest request);
        Task DeleteAsync(Guid id);
    }
}

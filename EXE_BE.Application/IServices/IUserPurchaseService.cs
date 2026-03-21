using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EXE_BE.Application.DTOs.Requests.UserPurchase;
using EXE_BE.Application.DTOs.Responses.UserPurchase;

namespace EXE_BE.Application.IServices
{
    public interface IUserPurchaseService
    {
        Task<IEnumerable<UserPurchaseResponse>> GetAllAsync();
        Task<UserPurchaseResponse?> GetByIdAsync(Guid id);
        Task AddAsync(CreateUserPurchaseRequest request);
        Task UpdateAsync(Guid id, UpdateUserPurchaseRequest request);
        Task DeleteAsync(Guid id);
    }
}

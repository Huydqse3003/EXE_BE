using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EXE_BE.Application.DTOs.Requests.UserCoinTransaction;
using EXE_BE.Application.DTOs.Responses.UserCoinTransaction;

namespace EXE_BE.Application.IServices
{
    public interface IUserCoinTransactionService
    {
        Task<IEnumerable<UserCoinTransactionResponse>> GetAllAsync();
        Task<UserCoinTransactionResponse?> GetByIdAsync(Guid id);
        Task AddAsync(CreateUserCoinTransactionRequest request);
        Task UpdateAsync(Guid id, UpdateUserCoinTransactionRequest request);
        Task DeleteAsync(Guid id);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EXE_BE.Application.DTOs.Requests.UserCoinTransaction;
using EXE_BE.Application.DTOs.Responses.UserCoinTransaction;
using EXE_BE.Application.IServices;
using EXE_BE.Domain.Entities;

namespace EXE_BE.Application.Services
{
    public class UserCoinTransactionService : IUserCoinTransactionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserCoinTransactionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<UserCoinTransactionResponse>> GetAllAsync()
        {
            var entities = await _unitOfWork.UserCoinTransactions.GetAllAsync();
            // TODO: Map from Entity to Response DTO
            return entities.Select(e => new UserCoinTransactionResponse()); 
        }

        public async Task<UserCoinTransactionResponse?> GetByIdAsync(Guid id)
        {
            var entity = await _unitOfWork.UserCoinTransactions.GetByIdAsync(id);
            if (entity == null) return null;
            
            // TODO: Map from Entity to Response DTO
            return new UserCoinTransactionResponse();
        }

        public async Task AddAsync(CreateUserCoinTransactionRequest request)
        {
            // TODO: Map from Request DTO to Entity
            var entity = new UserCoinTransaction();
            
            await _unitOfWork.UserCoinTransactions.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateAsync(Guid id, UpdateUserCoinTransactionRequest request)
        {
            var entity = await _unitOfWork.UserCoinTransactions.GetByIdAsync(id);
            if (entity != null)
            {
                // TODO: Apply updates from Request DTO to Entity

                _unitOfWork.UserCoinTransactions.Update(entity);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _unitOfWork.UserCoinTransactions.GetByIdAsync(id);
            if (entity != null)
            {
                _unitOfWork.UserCoinTransactions.Remove(entity);
                await _unitOfWork.SaveChangesAsync();
            }
        }
    }
}

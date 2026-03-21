using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EXE_BE.Application.DTOs.Requests.SubscriptionPackage;
using EXE_BE.Application.DTOs.Responses.SubscriptionPackage;
using EXE_BE.Application.IServices;
using EXE_BE.Domain.Entities;

namespace EXE_BE.Application.Services
{
    public class SubscriptionPackageService : ISubscriptionPackageService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SubscriptionPackageService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<SubscriptionPackageResponse>> GetAllAsync()
        {
            var entities = await _unitOfWork.SubscriptionPackages.GetAllAsync();
            // TODO: Map from Entity to Response DTO
            return entities.Select(e => new SubscriptionPackageResponse()); 
        }

        public async Task<SubscriptionPackageResponse?> GetByIdAsync(Guid id)
        {
            var entity = await _unitOfWork.SubscriptionPackages.GetByIdAsync(id);
            if (entity == null) return null;
            
            // TODO: Map from Entity to Response DTO
            return new SubscriptionPackageResponse();
        }

        public async Task AddAsync(CreateSubscriptionPackageRequest request)
        {
            // TODO: Map from Request DTO to Entity
            var entity = new SubscriptionPackage();
            
            await _unitOfWork.SubscriptionPackages.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateAsync(Guid id, UpdateSubscriptionPackageRequest request)
        {
            var entity = await _unitOfWork.SubscriptionPackages.GetByIdAsync(id);
            if (entity != null)
            {
                // TODO: Apply updates from Request DTO to Entity

                _unitOfWork.SubscriptionPackages.Update(entity);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _unitOfWork.SubscriptionPackages.GetByIdAsync(id);
            if (entity != null)
            {
                _unitOfWork.SubscriptionPackages.Remove(entity);
                await _unitOfWork.SaveChangesAsync();
            }
        }
    }
}

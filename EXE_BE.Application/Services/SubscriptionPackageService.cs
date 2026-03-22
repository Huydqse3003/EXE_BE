using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EXE_BE.Application.DTOs.Requests.SubscriptionPackage;
using EXE_BE.Application.DTOs.Responses;
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

        public async Task<ApiResponse> GetAllAsync()
        {
            var response = new ApiResponse();
            var entities = await _unitOfWork.SubscriptionPackages.GetAllAsync();
            return response.SetOk(entities.Select(MapToResponse));
        }

        public async Task<ApiResponse> GetByIdAsync(Guid id)
        {
            var response = new ApiResponse();
            var entity = await _unitOfWork.SubscriptionPackages.GetByIdAsync(id);
            if (entity == null)
            {
                return response.SetNotFound(message: "Không tìm thấy gói đăng ký.");
            }

            return response.SetOk(MapToResponse(entity));
        }

        public async Task<ApiResponse> AddAsync(CreateSubscriptionPackageRequest request)
        {
            var response = new ApiResponse();
            try
            {
                ValidateRequest(request.Name, request.Price, request.DurationDays);

            var entity = new SubscriptionPackage
            {
                PackageId = Guid.NewGuid(),
                Name = request.Name.Trim(),
                Description = request.Description.Trim(),
                Price = request.Price,
                DurationDays = request.DurationDays,
                AllowsPremiumCosmetics = request.AllowsPremiumCosmetics,
                RemovesAds = request.RemovesAds
            };

                await _unitOfWork.SubscriptionPackages.AddAsync(entity);
                await _unitOfWork.SaveChangesAsync();
                return response.SetOk(MapToResponse(entity));
            }
            catch (InvalidOperationException ex)
            {
                return response.SetBadRequest(message: ex.Message);
            }
        }

        public async Task<ApiResponse> UpdateAsync(Guid id, UpdateSubscriptionPackageRequest request)
        {
            var response = new ApiResponse();
            try
            {
                var entity = await _unitOfWork.SubscriptionPackages.GetByIdAsync(id);
                if (entity == null)
                {
                    return response.SetNotFound(message: "Không tìm thấy gói đăng ký.");
                }

                ValidateRequest(request.Name, request.Price, request.DurationDays);

                entity.Name = request.Name.Trim();
                entity.Description = request.Description.Trim();
                entity.Price = request.Price;
                entity.DurationDays = request.DurationDays;
                entity.AllowsPremiumCosmetics = request.AllowsPremiumCosmetics;
                entity.RemovesAds = request.RemovesAds;

                _unitOfWork.SubscriptionPackages.Update(entity);
                await _unitOfWork.SaveChangesAsync();
                return response.SetOk(MapToResponse(entity));
            }
            catch (InvalidOperationException ex)
            {
                return response.SetBadRequest(message: ex.Message);
            }
        }

        public async Task<ApiResponse> DeleteAsync(Guid id)
        {
            var response = new ApiResponse();
            var entity = await _unitOfWork.SubscriptionPackages.GetByIdAsync(id);
            if (entity == null)
            {
                return response.SetNotFound(message: "Không tìm thấy gói đăng ký.");
            }

            _unitOfWork.SubscriptionPackages.Remove(entity);
            await _unitOfWork.SaveChangesAsync();
            return response.SetOk("Xóa gói đăng ký thành công.");
        }

        private static SubscriptionPackageResponse MapToResponse(SubscriptionPackage entity)
        {
            return new SubscriptionPackageResponse
            {
                PackageId = entity.PackageId,
                Name = entity.Name,
                Description = entity.Description,
                Price = entity.Price,
                DurationDays = entity.DurationDays,
                AllowsPremiumCosmetics = entity.AllowsPremiumCosmetics,
                RemovesAds = entity.RemovesAds
            };
        }

        private static void ValidateRequest(string name, decimal price, int durationDays)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new InvalidOperationException("Tên gói không được để trống.");
            }

            if (price <= 0)
            {
                throw new InvalidOperationException("Giá gói phải lớn hơn 0.");
            }

            if (durationDays <= 0)
            {
                throw new InvalidOperationException("Thời hạn gói phải lớn hơn 0 ngày.");
            }
        }
    }
}

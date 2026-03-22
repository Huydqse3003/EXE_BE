using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EXE_BE.Application.DTOs.Requests.UserSubscription;
using EXE_BE.Application.DTOs.Responses;
using EXE_BE.Application.DTOs.Responses.UserSubscription;
using EXE_BE.Application.IServices;
using EXE_BE.Domain.Entities;

namespace EXE_BE.Application.Services
{
    public class UserSubscriptionService : IUserSubscriptionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserSubscriptionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse> GetAllAsync()
        {
            var response = new ApiResponse();
            var entities = await _unitOfWork.UserSubscriptions.GetAllAsync();
            return response.SetOk(entities.Select(MapToResponse));
        }

        public async Task<ApiResponse> GetByIdAsync(Guid id)
        {
            var response = new ApiResponse();
            var entity = await _unitOfWork.UserSubscriptions.GetByIdAsync(id);
            if (entity == null)
            {
                return response.SetNotFound(message: "Không tìm thấy gói người dùng.");
            }

            return response.SetOk(MapToResponse(entity));
        }

        public async Task<ApiResponse> AddAsync(CreateUserSubscriptionRequest request)
        {
            var response = new ApiResponse();
            try
            {
                ValidateDates(request.StartDate, request.EndDate);

            var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);
            if (user == null)
            {
                return response.SetNotFound(message: "Không tìm thấy người dùng.");
            }

            var package = await _unitOfWork.SubscriptionPackages.GetByIdAsync(request.PackageId);
            if (package == null)
            {
                return response.SetNotFound(message: "Không tìm thấy gói đăng ký.");
            }

            var entity = new UserSubscription
            {
                UserSubscriptionId = Guid.NewGuid(),
                UserId = user.UserId,
                PackageId = package.PackageId,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                IsActive = request.IsActive
            };

                await _unitOfWork.UserSubscriptions.AddAsync(entity);
                await _unitOfWork.SaveChangesAsync();
                return response.SetOk(MapToResponse(entity));
            }
            catch (InvalidOperationException ex)
            {
                return response.SetBadRequest(message: ex.Message);
            }
        }

        public async Task<ApiResponse> UpdateAsync(Guid id, UpdateUserSubscriptionRequest request)
        {
            var response = new ApiResponse();
            try
            {
                var entity = await _unitOfWork.UserSubscriptions.GetByIdAsync(id);
                if (entity == null)
                {
                    return response.SetNotFound(message: "Không tìm thấy gói người dùng.");
                }

                ValidateDates(request.StartDate, request.EndDate);

                entity.StartDate = request.StartDate;
                entity.EndDate = request.EndDate;
                entity.IsActive = request.IsActive;

                _unitOfWork.UserSubscriptions.Update(entity);
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
            var entity = await _unitOfWork.UserSubscriptions.GetByIdAsync(id);
            if (entity == null)
            {
                return response.SetNotFound(message: "Không tìm thấy gói người dùng.");
            }

            _unitOfWork.UserSubscriptions.Remove(entity);
            await _unitOfWork.SaveChangesAsync();
            return response.SetOk("Xóa gói người dùng thành công.");
        }

        private static UserSubscriptionResponse MapToResponse(UserSubscription entity)
        {
            return new UserSubscriptionResponse
            {
                UserSubscriptionId = entity.UserSubscriptionId,
                UserId = entity.UserId,
                PackageId = entity.PackageId,
                StartDate = entity.StartDate,
                EndDate = entity.EndDate,
                IsActive = entity.IsActive
            };
        }

        private static void ValidateDates(DateTime startDate, DateTime endDate)
        {
            if (endDate <= startDate)
            {
                throw new InvalidOperationException("Ngày hết hạn phải lớn hơn ngày bắt đầu.");
            }
        }
    }
}

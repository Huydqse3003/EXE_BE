using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EXE_BE.Application.DTOs.Requests.Friendship;
using EXE_BE.Application.DTOs.Responses;
using EXE_BE.Application.DTOs.Responses.Friendship;
using EXE_BE.Application.IServices;
using EXE_BE.Domain.Entities;

namespace EXE_BE.Application.Services
{
    public class FriendshipService : IFriendshipService
    {
        private readonly IUnitOfWork _unitOfWork;

        public FriendshipService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse> GetAllAsync()
        {
            var response = new ApiResponse();
            var entities = await _unitOfWork.Friendships.GetAllAsync();
            return response.SetOk(entities.Select(Map));
        }

        public async Task<ApiResponse> GetByIdAsync(Guid id)
        {
            var response = new ApiResponse();
            var entity = await _unitOfWork.Friendships.GetByIdAsync(id);
            if (entity == null)
            {
                return response.SetNotFound(message: "Không tìm thấy lời mời kết bạn.");
            }

            return response.SetOk(Map(entity));
        }

        public async Task<ApiResponse> AddAsync(CreateFriendshipRequest request)
        {
            var response = new ApiResponse();

            if (request.UserId == request.FriendId)
            {
                return response.SetBadRequest(message: "Không thể gửi lời mời cho chính mình.");
            }

            var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);
            if (user == null)
            {
                return response.SetNotFound(message: "Không tìm thấy người gửi lời mời.");
            }

            var friend = await _unitOfWork.Users.GetByIdAsync(request.FriendId);
            if (friend == null)
            {
                return response.SetNotFound(message: "Không tìm thấy người nhận lời mời.");
            }

            var existed = await _unitOfWork.Friendships.FindAsync(f =>
                (f.UserId == request.UserId && f.FriendId == request.FriendId) ||
                (f.UserId == request.FriendId && f.FriendId == request.UserId));

            if (existed.Any(f => string.Equals(f.Status, "Accepted", StringComparison.OrdinalIgnoreCase)))
            {
                return response.SetBadRequest(message: "Hai người đã là bạn bè.");
            }

            if (existed.Any(f => string.Equals(f.Status, "Pending", StringComparison.OrdinalIgnoreCase)))
            {
                return response.SetBadRequest(message: "Lời mời kết bạn đang chờ xử lý.");
            }

            var entity = new Friendship
            {
                FriendshipId = Guid.NewGuid(),
                UserId = request.UserId,
                FriendId = request.FriendId,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Friendships.AddAsync(entity);

            await _unitOfWork.UserHabits.AddAsync(new UserHabit
            {
                HabitId = Guid.NewGuid(),
                UserId = user.UserId,
                HabitType = "FriendRequestSent",
                EventTime = DateTime.UtcNow,
                Details = $"Send friend request to {friend.UserId}"
            });

            await _unitOfWork.SaveChangesAsync();

            return response.SetOk(Map(entity));
        }

        public async Task<ApiResponse> UpdateAsync(Guid id, UpdateFriendshipRequest request)
        {
            var response = new ApiResponse();
            var entity = await _unitOfWork.Friendships.GetByIdAsync(id);
            if (entity == null)
            {
                return response.SetNotFound(message: "Không tìm thấy lời mời kết bạn.");
            }

            if (string.IsNullOrWhiteSpace(request.Status))
            {
                return response.SetBadRequest(message: "Status không được để trống.");
            }

            var allowedStatuses = new[] { "Pending", "Accepted", "Rejected", "Blocked" };
            if (!allowedStatuses.Contains(request.Status, StringComparer.OrdinalIgnoreCase))
            {
                return response.SetBadRequest(message: "Status không hợp lệ.");
            }

            entity.Status = request.Status;
            _unitOfWork.Friendships.Update(entity);

            await _unitOfWork.UserHabits.AddAsync(new UserHabit
            {
                HabitId = Guid.NewGuid(),
                UserId = entity.FriendId,
                HabitType = "FriendRequestUpdated",
                EventTime = DateTime.UtcNow,
                Details = $"Update friend request {entity.FriendshipId} to {entity.Status}"
            });

            await _unitOfWork.SaveChangesAsync();

            return response.SetOk(Map(entity));
        }

        public async Task<ApiResponse> GetPendingByUserIdAsync(Guid userId)
        {
            var response = new ApiResponse();
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
            {
                return response.SetNotFound(message: "Không tìm thấy người dùng.");
            }

            var entities = await _unitOfWork.Friendships.FindAsync(f =>
                f.FriendId == user.UserId && string.Equals(f.Status, "Pending", StringComparison.OrdinalIgnoreCase));

            return response.SetOk(entities.Select(Map));
        }

        public async Task<ApiResponse> DeleteAsync(Guid id)
        {
            var response = new ApiResponse();
            var entity = await _unitOfWork.Friendships.GetByIdAsync(id);
            if (entity == null)
            {
                return response.SetNotFound(message: "Không tìm thấy lời mời kết bạn.");
            }

            _unitOfWork.Friendships.Remove(entity);
            await _unitOfWork.SaveChangesAsync();
            return response.SetOk("Xóa lời mời kết bạn thành công.");
        }

        private static FriendshipResponse Map(Friendship entity)
        {
            return new FriendshipResponse
            {
                FriendshipId = entity.FriendshipId,
                UserId = entity.UserId,
                FriendId = entity.FriendId,
                Status = entity.Status,
                CreatedAt = entity.CreatedAt
            };
        }
    }
}

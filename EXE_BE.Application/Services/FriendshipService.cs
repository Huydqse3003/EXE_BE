using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EXE_BE.Application.DTOs.Requests.Friendship;
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

        public async Task<IEnumerable<FriendshipResponse>> GetAllAsync()
        {
            var entities = await _unitOfWork.Friendships.GetAllAsync();
            return entities.Select(Map);
        }

        public async Task<FriendshipResponse?> GetByIdAsync(Guid id)
        {
            var entity = await _unitOfWork.Friendships.GetByIdAsync(id);
            if (entity == null) return null;
            return Map(entity);
        }

        public async Task<FriendshipResponse> AddAsync(CreateFriendshipRequest request)
        {
            if (request.UserId == request.FriendId)
            {
                throw new InvalidOperationException("Không thể gửi lời mời cho chính mình.");
            }

            var user = await _unitOfWork.Users.GetByIdAsync(request.UserId)
                ?? throw new KeyNotFoundException("Không tìm thấy người gửi lời mời.");

            var friend = await _unitOfWork.Users.GetByIdAsync(request.FriendId)
                ?? throw new KeyNotFoundException("Không tìm thấy người nhận lời mời.");

            var existed = await _unitOfWork.Friendships.FindAsync(f =>
                (f.UserId == request.UserId && f.FriendId == request.FriendId) ||
                (f.UserId == request.FriendId && f.FriendId == request.UserId));

            if (existed.Any(f => string.Equals(f.Status, "Accepted", StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException("Hai người đã là bạn bè.");
            }

            if (existed.Any(f => string.Equals(f.Status, "Pending", StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException("Lời mời kết bạn đang chờ xử lý.");
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

            return Map(entity);
        }

        public async Task<FriendshipResponse?> UpdateAsync(Guid id, UpdateFriendshipRequest request)
        {
            var entity = await _unitOfWork.Friendships.GetByIdAsync(id);
            if (entity == null)
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(request.Status))
            {
                throw new InvalidOperationException("Status không được để trống.");
            }

            var allowedStatuses = new[] { "Pending", "Accepted", "Rejected", "Blocked" };
            if (!allowedStatuses.Contains(request.Status, StringComparer.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Status không hợp lệ.");
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

            return Map(entity);
        }

        public async Task<IEnumerable<FriendshipResponse>> GetPendingByUserIdAsync(Guid userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId)
                ?? throw new KeyNotFoundException("Không tìm thấy người dùng.");

            var entities = await _unitOfWork.Friendships.FindAsync(f =>
                f.FriendId == user.UserId && string.Equals(f.Status, "Pending", StringComparison.OrdinalIgnoreCase));

            return entities.Select(Map);
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _unitOfWork.Friendships.GetByIdAsync(id);
            if (entity != null)
            {
                _unitOfWork.Friendships.Remove(entity);
                await _unitOfWork.SaveChangesAsync();
            }
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

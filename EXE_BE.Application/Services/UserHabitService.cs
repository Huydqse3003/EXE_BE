using EXE_BE.Application.DTOs.Requests.UserHabit;
using EXE_BE.Application.DTOs.Responses;
using EXE_BE.Application.DTOs.Responses.UserHabit;
using EXE_BE.Application.IServices;
using EXE_BE.Domain.Entities;

namespace EXE_BE.Application.Services
{
    public class UserHabitService : IUserHabitService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserHabitService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse> LogHabitAsync(CreateUserHabitRequest request)
        {
            var response = new ApiResponse();

            if (string.IsNullOrWhiteSpace(request.HabitType))
            {
                return response.SetBadRequest(message: "HabitType không được để trống.");
            }

            var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);
            if (user == null)
            {
                return response.SetNotFound(message: "Không tìm thấy người dùng.");
            }

            var entity = new UserHabit
            {
                HabitId = Guid.NewGuid(),
                UserId = user.UserId,
                HabitType = request.HabitType,
                EventTime = request.EventTime ?? DateTime.UtcNow,
                LoginTime = request.LoginTime,
                LogoutTime = request.LogoutTime,
                Details = request.Details ?? string.Empty,
                MetadataJson = request.MetadataJson
            };

            await _unitOfWork.UserHabits.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return response.SetOk(Map(entity));
        }

        public async Task<ApiResponse> GetByUserIdAsync(Guid userId)
        {
            var response = new ApiResponse();

            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
            {
                return response.SetNotFound(message: "Không tìm thấy người dùng.");
            }

            var habits = await _unitOfWork.UserHabits.FindAsync(h => h.UserId == user.UserId);

            return response.SetOk(habits
                .OrderByDescending(h => h.EventTime)
                .Select(Map));
        }

        public async Task<ApiResponse> GetSummaryByUserIdAsync(Guid userId)
        {
            var response = new ApiResponse();
            var habitsResponse = await GetByUserIdAsync(userId);

            if (!habitsResponse.IsSuccess)
            {
                return habitsResponse;
            }

            var allHabits = ((IEnumerable<UserHabitResponse>?)habitsResponse.Result)?.ToList() ?? [];

            return response.SetOk(new UserHabitSummaryResponse
            {
                UserId = userId,
                TotalHabits = allHabits.Count,
                TotalLogins = allHabits.Count(h => h.HabitType == "Login"),
                TotalFocusSessions = allHabits.Count(h => h.HabitType == "FocusStart" || h.HabitType == "FocusCompleted"),
                TotalPurchases = allHabits.Count(h => h.HabitType == "Purchase"),
                TotalRoomDecorations = allHabits.Count(h => h.HabitType == "RoomDecoration"),
                LastLoginTime = allHabits
                    .Where(h => h.HabitType == "Login")
                    .OrderByDescending(h => h.LoginTime ?? h.EventTime)
                    .Select(h => h.LoginTime ?? h.EventTime)
                    .FirstOrDefault(),
                LastEventTime = allHabits
                    .OrderByDescending(h => h.EventTime)
                    .Select(h => (DateTime?)h.EventTime)
                    .FirstOrDefault()
            });
        }

        private static UserHabitResponse Map(UserHabit entity)
        {
            return new UserHabitResponse
            {
                HabitId = entity.HabitId,
                UserId = entity.UserId,
                HabitType = entity.HabitType,
                EventTime = entity.EventTime,
                LoginTime = entity.LoginTime,
                LogoutTime = entity.LogoutTime,
                Details = entity.Details,
                MetadataJson = entity.MetadataJson
            };
        }
    }
}

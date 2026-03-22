using EXE_BE.Application.DTOs.Requests.UserHabit;
using EXE_BE.Application.DTOs.Responses;
using EXE_BE.Application.DTOs.Responses.UserHabit;

namespace EXE_BE.Application.IServices
{
    public interface IUserHabitService
    {
        Task<ApiResponse> LogHabitAsync(CreateUserHabitRequest request);
        Task<ApiResponse> GetByUserIdAsync(Guid userId);
        Task<ApiResponse> GetSummaryByUserIdAsync(Guid userId);
    }
}

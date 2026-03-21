using EXE_BE.Application.DTOs.Requests.UserHabit;
using EXE_BE.Application.DTOs.Responses.UserHabit;

namespace EXE_BE.Application.IServices
{
    public interface IUserHabitService
    {
        Task<UserHabitResponse> LogHabitAsync(CreateUserHabitRequest request);
        Task<IEnumerable<UserHabitResponse>> GetByUserIdAsync(Guid userId);
        Task<UserHabitSummaryResponse> GetSummaryByUserIdAsync(Guid userId);
    }
}

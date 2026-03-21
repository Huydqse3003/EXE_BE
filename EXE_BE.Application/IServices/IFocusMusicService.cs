using EXE_BE.Application.DTOs.Requests.FocusMusic;
using EXE_BE.Application.DTOs.Responses.FocusMusic;

namespace EXE_BE.Application.IServices
{
    public interface IFocusMusicService
    {
        Task<FocusMusicResponse> AddAsync(CreateFocusMusicRequest request);
        Task<IEnumerable<FocusMusicResponse>> GetByUserIdAsync(Guid userId);
        Task<FocusMusicResponse?> GetCurrentByUserIdAsync(Guid userId);
        Task<FocusMusicResponse> SetCurrentAsync(Guid userId, Guid musicId);
    }
}

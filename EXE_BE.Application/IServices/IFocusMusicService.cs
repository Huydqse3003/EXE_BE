using EXE_BE.Application.DTOs.Requests.FocusMusic;
using EXE_BE.Application.DTOs.Responses;
using EXE_BE.Application.DTOs.Responses.FocusMusic;

namespace EXE_BE.Application.IServices
{
    public interface IFocusMusicService
    {
        Task<ApiResponse> AddAsync(CreateFocusMusicRequest request);
        Task<ApiResponse> GetByUserIdAsync(Guid userId);
        Task<ApiResponse> GetCurrentByUserIdAsync(Guid userId);
        Task<ApiResponse> SetCurrentAsync(Guid userId, Guid musicId);
    }
}

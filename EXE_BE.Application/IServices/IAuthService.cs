using EXE_BE.Application.DTOs.Requests.Auth;
using EXE_BE.Application.DTOs.Responses;

namespace EXE_BE.Application.IServices
{
    public interface IAuthService
    {
        Task<ApiResponse> RegisterAsync(RegisterRequest request);
        Task<ApiResponse> LoginAsync(LoginRequest request);
        Task<ApiResponse> ResetPasswordAsync(ResetPasswordRequest request);
    }
}

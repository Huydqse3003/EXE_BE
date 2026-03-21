using EXE_BE.Application.DTOs.Requests.Auth;
using EXE_BE.Application.DTOs.Responses.Auth;

namespace EXE_BE.Application.IServices
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(RegisterRequest request);
        Task<AuthResponse> LoginAsync(LoginRequest request);
        Task<AuthResponse> ResetPasswordAsync(ResetPasswordRequest request);
    }
}

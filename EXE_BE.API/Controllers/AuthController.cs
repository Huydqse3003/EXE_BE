using EXE_BE.Application.DTOs.Requests.Auth;
using EXE_BE.Application.DTOs.Responses;
using EXE_BE.Application.DTOs.Responses.Auth;
using EXE_BE.API.Hubs;
using EXE_BE.Application.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace EXE_BE.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IHubContext<RealtimeHub> _hubContext;

        public AuthController(IAuthService authService, IHubContext<RealtimeHub> hubContext)
        {
            _authService = authService;
            _hubContext = hubContext;
        }

        /// <summary>
        /// Đăng ký tài khoản mới.
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(new ApiResponse().SetBadRequest(message: string.Join("; ", errors)));
            }

            var response = await _authService.RegisterAsync(request);

            if (response.IsSuccess && response.Result is AuthResponse authResponse)
            {
                await _hubContext.Clients.Group($"user:{authResponse.UserId}")
                    .SendAsync("UserRegistered", authResponse);
            }

            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        /// <summary>
        /// Đăng nhập bằng thông tin tài khoản.
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(new ApiResponse().SetBadRequest(message: string.Join("; ", errors)));
            }

            var response = await _authService.LoginAsync(request);

            if (response.IsSuccess && response.Result is AuthResponse authResponse)
            {
                await _hubContext.Clients.Group($"user:{authResponse.UserId}")
                    .SendAsync("UserLoggedIn", authResponse);
            }

            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        /// <summary>
        /// Đặt lại mật khẩu cho tài khoản.
        /// </summary>
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(new ApiResponse().SetBadRequest(message: string.Join("; ", errors)));
            }

            var response = await _authService.ResetPasswordAsync(request);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }
    }
}

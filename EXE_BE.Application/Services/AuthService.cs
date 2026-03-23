using EXE_BE.Application.DTOs.Requests.Auth;
using EXE_BE.Application.DTOs.Responses;
using EXE_BE.Application.DTOs.Responses.Auth;
using EXE_BE.Application.IServices;
using EXE_BE.Application.Security;
using EXE_BE.Domain;
using EXE_BE.Domain.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EXE_BE.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly AppSettings _appSettings;
        private readonly JwtSettings _jwtSettings;

        public AuthService(IUnitOfWork unitOfWork, AppSettings appSettings)
        {
            _unitOfWork = unitOfWork;
            _appSettings = appSettings;
            _jwtSettings = _appSettings.JwtSettings;
        }

        public async Task<ApiResponse> RegisterAsync(RegisterRequest request)
        {
            var response = new ApiResponse();

            if (string.IsNullOrWhiteSpace(request.Username) ||
                string.IsNullOrWhiteSpace(request.Email) ||
                request.Age <= 0 ||
                string.IsNullOrWhiteSpace(request.PhoneNumber) ||
                string.IsNullOrWhiteSpace(request.Password) ||
                string.IsNullOrWhiteSpace(request.ConfirmPassword))
            {
                return response.SetBadRequest(message: "Username, email, tuổi, số điện thoại, password, confirm password không được để trống.");
            }

            if (request.Age < 10 || request.Age > 100)
            {
                return response.SetBadRequest(message: "Tuổi không hợp lệ.");
            }

            if (request.Password != request.ConfirmPassword)
            {
                return response.SetBadRequest(message: "Password và confirm password không khớp.");
            }

            var existedByUsername = await _unitOfWork.Users.FindAsync(u => u.Username == request.Username);
            if (existedByUsername.Any())
            {
                return response.SetBadRequest(message: "Username đã tồn tại.");
            }

            var existedByEmail = await _unitOfWork.Users.FindAsync(u => u.Email == request.Email);
            if (existedByEmail.Any())
            {
                return response.SetBadRequest(message: "Email đã tồn tại.");
            }

            var user = new User
            {
                UserId = Guid.NewGuid(),
                Username = request.Username.Trim(),
                Email = request.Email.Trim(),
                Age = request.Age,
                PhoneNumber = request.PhoneNumber.Trim(),
                PasswordHash = PasswordHasher.Hash(request.Password),
                Level = 1,
                ExperiencePoints = 0,
                Coins = 0
            };

            await _unitOfWork.Users.AddAsync(user);

            await _unitOfWork.UserHabits.AddAsync(new UserHabit
            {
                HabitId = Guid.NewGuid(),
                UserId = user.UserId,
                HabitType = "Register",
                EventTime = DateTime.UtcNow,
                Details = "User register account"
            });

            await _unitOfWork.SaveChangesAsync();

            var (accessToken, accessTokenExpiresAt) = CreateToken(user);

            var authResponse = new AuthResponse
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                Age = user.Age,
                PhoneNumber = user.PhoneNumber,
                AccessToken = accessToken,
                AccessTokenExpiresAt = accessTokenExpiresAt,
                Message = "Đăng ký thành công."
            };

            return response.SetOk(authResponse);
        }

        public async Task<ApiResponse> LoginAsync(LoginRequest request)
        {
            var response = new ApiResponse();

            if (string.IsNullOrWhiteSpace(request.UsernameOrEmail) || string.IsNullOrWhiteSpace(request.Password))
            {
                return response.SetBadRequest(message: "Username/email và password không được để trống.");
            }

            var user = (await _unitOfWork.Users.FindAsync(u =>
                u.Username == request.UsernameOrEmail || u.Email == request.UsernameOrEmail)).FirstOrDefault();

            if (user == null || !PasswordHasher.Verify(request.Password, user.PasswordHash))
            {
                return response.SetBadRequest(message: "Sai tài khoản hoặc mật khẩu.");
            }

            await _unitOfWork.UserHabits.AddAsync(new UserHabit
            {
                HabitId = Guid.NewGuid(),
                UserId = user.UserId,
                HabitType = "Login",
                EventTime = DateTime.UtcNow,
                LoginTime = DateTime.UtcNow,
                Details = "User login"
            });

            await _unitOfWork.SaveChangesAsync();

            var (accessToken, accessTokenExpiresAt) = CreateToken(user);

            var authResponse = new AuthResponse
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                Age = user.Age,
                PhoneNumber = user.PhoneNumber,
                AccessToken = accessToken,
                AccessTokenExpiresAt = accessTokenExpiresAt,
                Message = "Đăng nhập thành công."
            };

            return response.SetOk(authResponse);
        }

        public async Task<ApiResponse> ResetPasswordAsync(ResetPasswordRequest request)
        {
            var response = new ApiResponse();

            if (string.IsNullOrWhiteSpace(request.UsernameOrEmail) ||
                string.IsNullOrWhiteSpace(request.NewPassword) ||
                string.IsNullOrWhiteSpace(request.ConfirmPassword))
            {
                return response.SetBadRequest(message: "Username/email, mật khẩu mới và confirm password không được để trống.");
            }

            if (request.NewPassword != request.ConfirmPassword)
            {
                return response.SetBadRequest(message: "Mật khẩu mới và confirm password không khớp.");
            }

            var user = (await _unitOfWork.Users.FindAsync(u =>
                u.Username == request.UsernameOrEmail || u.Email == request.UsernameOrEmail)).FirstOrDefault();

            if (user == null)
            {
                return response.SetBadRequest(message: "Không tìm thấy tài khoản.");
            }

            user.PasswordHash = PasswordHasher.Hash(request.NewPassword);
            _unitOfWork.Users.Update(user);

            await _unitOfWork.UserHabits.AddAsync(new UserHabit
            {
                HabitId = Guid.NewGuid(),
                UserId = user.UserId,
                HabitType = "ResetPassword",
                EventTime = DateTime.UtcNow,
                Details = "User reset password"
            });

            await _unitOfWork.SaveChangesAsync();

            var (accessToken, accessTokenExpiresAt) = CreateToken(user);

            var authResponse = new AuthResponse
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                Age = user.Age,
                PhoneNumber = user.PhoneNumber,
                AccessToken = accessToken,
                AccessTokenExpiresAt = accessTokenExpiresAt,
                Message = "Đặt lại mật khẩu thành công."
            };

            return response.SetOk(authResponse);
        }

        private (string AccessToken, DateTime ExpiresAt) CreateToken(User user)
        {
            if (string.IsNullOrWhiteSpace(_jwtSettings.Secret) || _jwtSettings.Secret.Length < 32)
            {
                throw new InvalidOperationException("JwtSettings:Secret phải có ít nhất 32 ký tự.");
            }

            var expiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryInMinutes <= 0 ? 60 : _jwtSettings.ExpiryInMinutes);
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new(ClaimTypes.Name, user.Username),
                new(ClaimTypes.Email, user.Email),
                new("UserId", user.UserId.ToString()),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: string.IsNullOrWhiteSpace(_jwtSettings.Issuer) ? null : _jwtSettings.Issuer,
                audience: string.IsNullOrWhiteSpace(_jwtSettings.Audience) ? null : _jwtSettings.Audience,
                claims: claims,
                expires: expiresAt,
                signingCredentials: credentials);

            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
            return (accessToken, expiresAt);
        }
    }
}

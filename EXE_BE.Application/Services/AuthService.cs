using EXE_BE.Application.DTOs.Requests.Auth;
using EXE_BE.Application.DTOs.Responses.Auth;
using EXE_BE.Application.IServices;
using EXE_BE.Application.Security;
using EXE_BE.Domain.Entities;

namespace EXE_BE.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AuthService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Username) ||
                string.IsNullOrWhiteSpace(request.Email) ||
                request.Age <= 0 ||
                string.IsNullOrWhiteSpace(request.PhoneNumber) ||
                string.IsNullOrWhiteSpace(request.Password) ||
                string.IsNullOrWhiteSpace(request.ConfirmPassword))
            {
                throw new InvalidOperationException("Username, email, tuổi, số điện thoại, password, confirm password không được để trống.");
            }

            if (request.Age < 10 || request.Age > 100)
            {
                throw new InvalidOperationException("Tuổi không hợp lệ.");
            }

            if (request.Password != request.ConfirmPassword)
            {
                throw new InvalidOperationException("Password và confirm password không khớp.");
            }

            var existedByUsername = await _unitOfWork.Users.FindAsync(u => u.Username == request.Username);
            if (existedByUsername.Any())
            {
                throw new InvalidOperationException("Username đã tồn tại.");
            }

            var existedByEmail = await _unitOfWork.Users.FindAsync(u => u.Email == request.Email);
            if (existedByEmail.Any())
            {
                throw new InvalidOperationException("Email đã tồn tại.");
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

            return new AuthResponse
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                Age = user.Age,
                PhoneNumber = user.PhoneNumber,
                Message = "Đăng ký thành công."
            };
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.UsernameOrEmail) || string.IsNullOrWhiteSpace(request.Password))
            {
                throw new InvalidOperationException("Username/email và password không được để trống.");
            }

            var user = (await _unitOfWork.Users.FindAsync(u =>
                u.Username == request.UsernameOrEmail || u.Email == request.UsernameOrEmail)).FirstOrDefault();

            if (user == null || !PasswordHasher.Verify(request.Password, user.PasswordHash))
            {
                throw new InvalidOperationException("Sai tài khoản hoặc mật khẩu.");
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

            return new AuthResponse
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                Age = user.Age,
                PhoneNumber = user.PhoneNumber,
                Message = "Đăng nhập thành công."
            };
        }

        public async Task<AuthResponse> ResetPasswordAsync(ResetPasswordRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.UsernameOrEmail) ||
                string.IsNullOrWhiteSpace(request.NewPassword) ||
                string.IsNullOrWhiteSpace(request.ConfirmPassword))
            {
                throw new InvalidOperationException("Username/email, mật khẩu mới và confirm password không được để trống.");
            }

            if (request.NewPassword != request.ConfirmPassword)
            {
                throw new InvalidOperationException("Mật khẩu mới và confirm password không khớp.");
            }

            var user = (await _unitOfWork.Users.FindAsync(u =>
                u.Username == request.UsernameOrEmail || u.Email == request.UsernameOrEmail)).FirstOrDefault();

            if (user == null)
            {
                throw new InvalidOperationException("Không tìm thấy tài khoản.");
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

            return new AuthResponse
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                Age = user.Age,
                PhoneNumber = user.PhoneNumber,
                Message = "Đặt lại mật khẩu thành công."
            };
        }
    }
}

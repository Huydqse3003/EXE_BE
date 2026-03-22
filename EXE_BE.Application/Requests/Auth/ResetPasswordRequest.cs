namespace EXE_BE.Application.DTOs.Requests.Auth
{
    public class ResetPasswordRequest
    {
        public string UsernameOrEmail { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}

namespace EXE_BE.Application.DTOs.Auth
{
    public class ClaimDTO
    {
        public Guid UserId { get; set; }
        public string Role { get; set; } = string.Empty;
    }
}

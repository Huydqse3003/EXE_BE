using System.Security.Claims;
using EXE_BE.Application.DTOs.Auth;
using EXE_BE.Application.IServices;

namespace EXE_BE.API.Services
{
    public class ClaimService : IClaimService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ClaimService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public ClaimDTO GetUserClaim()
        {
            var principal = _httpContextAccessor.HttpContext?.User;
            if (principal?.Identity?.IsAuthenticated != true)
            {
                throw new InvalidOperationException("User is not authenticated.");
            }

            var userIdClaim = principal.FindFirst("UserId")?.Value
                ?? principal.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? principal.FindFirst("sub")?.Value;

            if (string.IsNullOrWhiteSpace(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                throw new InvalidOperationException("UserId claim is missing or invalid.");
            }

            var role = principal.FindFirst(ClaimTypes.Role)?.Value
                ?? principal.FindFirst("Role")?.Value
                ?? string.Empty;

            return new ClaimDTO
            {
                UserId = userId,
                Role = role
            };
        }
    }
}

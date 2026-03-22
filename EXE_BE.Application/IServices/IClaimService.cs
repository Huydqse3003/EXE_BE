using EXE_BE.Application.DTOs.Auth;

namespace EXE_BE.Application.IServices
{
    public interface IClaimService
    {
        ClaimDTO GetUserClaim();
    }
}

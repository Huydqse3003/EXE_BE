namespace EXE_BE.Application.DTOs.Requests.Payment
{
    public class CreatePayOSPaymentLinkRequest
    {
        public Guid UserId { get; set; }
        public Guid PackageId { get; set; }
        public string? ReturnUrl { get; set; }
        public string? CancelUrl { get; set; }
    }
}

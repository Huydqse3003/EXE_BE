namespace EXE_BE.Application.DTOs.Requests.Payment
{
    public class ConfirmPayOSPaymentRequest
    {
        public Guid UserId { get; set; }
        public Guid PackageId { get; set; }
        public long OrderCode { get; set; }
        public bool IsPaid { get; set; }
        public string? Status { get; set; }
        public string? TransactionId { get; set; }
    }
}

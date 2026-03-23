namespace EXE_BE.Application.DTOs.Responses.Payment
{
    public class ConfirmPayOSPaymentResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public Guid? UserSubscriptionId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public long? OrderCode { get; set; }
    }
}

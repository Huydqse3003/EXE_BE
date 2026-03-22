namespace EXE_BE.Application.DTOs.Responses.Payment
{
    public class CreatePayOSPaymentLinkResponse
    {
        public long OrderCode { get; set; }
        public decimal Amount { get; set; }
        public string CheckoutUrl { get; set; } = string.Empty;
        public string QrCode { get; set; } = string.Empty;
        public string PaymentLinkId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}

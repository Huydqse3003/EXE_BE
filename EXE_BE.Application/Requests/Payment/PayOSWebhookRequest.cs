namespace EXE_BE.Application.DTOs.Requests.Payment
{
    public class PayOSWebhookRequest
    {
        public string? Code { get; set; }
        public string? Desc { get; set; }
        public bool Success { get; set; }
        public string? Signature { get; set; }
        public PayOSWebhookData? Data { get; set; }
    }

    public class PayOSWebhookData
    {
        public long OrderCode { get; set; }
        public string? Status { get; set; }
        public string? PaymentLinkId { get; set; }
        public string? Reference { get; set; }
    }
}

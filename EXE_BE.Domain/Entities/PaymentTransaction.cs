namespace EXE_BE.Domain.Entities
{
    public class PaymentTransaction : Base
    {
        public Guid PaymentTransactionId { get; set; }
        public long OrderCode { get; set; }
        public Guid UserId { get; set; }
        public Guid PackageId { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? PaymentLinkId { get; set; }
        public string? TransactionId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? PaidAt { get; set; }
        public string? InvoiceFilePath { get; set; }

        public virtual User? User { get; set; }
        public virtual SubscriptionPackage? SubscriptionPackage { get; set; }
    }
}

namespace EXE_BE.Application.DTOs.Requests.UserSubscription
{
    public class CreateUserSubscriptionRequest
    {
        public Guid UserId { get; set; }
        public Guid PackageId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
    }
}

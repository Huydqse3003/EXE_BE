using System;

namespace EXE_BE.Application.DTOs.Responses.UserSubscription
{
    public class UserSubscriptionResponse
    {
        public Guid UserSubscriptionId { get; set; }
        public Guid UserId { get; set; }
        public Guid PackageId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
    }
}

using System;

namespace EXE_BE.Application.DTOs.Requests.UserSubscription
{
    public class UpdateUserSubscriptionRequest
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
    }
}

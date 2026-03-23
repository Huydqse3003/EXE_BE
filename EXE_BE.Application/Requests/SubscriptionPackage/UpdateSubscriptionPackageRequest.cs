using System;

namespace EXE_BE.Application.DTOs.Requests.SubscriptionPackage
{
    public class UpdateSubscriptionPackageRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int DurationDays { get; set; }
        public bool AllowsPremiumCosmetics { get; set; }
        public bool RemovesAds { get; set; }
    }
}

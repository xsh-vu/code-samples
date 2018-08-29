using System;
using System.ComponentModel.DataAnnotations;

namespace Models.Domain
{
    public class UserSubscriptions
    {
        [Required]
        public string CustomerId { get; set; }
        [Required]
        public string CustomerName { get; set; }
        [Required]
        public string CustomerEmail { get; set; }
        [Required]
        public string PlanName { get; set; }
        [Required]
        public string DurationName { get; set; }
        [Required]
        public double DiscountPercent { get; set; }
        [Required]
        public double Price { get; set; }
        [Required]
        public string Currency { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime NextBillingDate { get; set; }
        [Required]
        public int UserBaseId { get; set; }
        [Required]
        public int UserProfileId { get; set; }
        [Required]
        public bool IsActive { get; set; }
        [Required]
        public string CurrentCard { get; set; }
        [Required]
        public int CardExpMonth { get; set; }
        [Required]
        public int CardExpYear { get; set; }
    }
}
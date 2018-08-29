using System;
using System.ComponentModel.DataAnnotations;

namespace Models.Requests
{
    public class ChargeRequest
    {
        [Required]
        public int UserBaseId { get; set; }
        [Required]
        public double Amount { get; set; }
        [Required]
        public string Currency { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string SourceTokenOrExistingSourceId { get; set; }
        [Required]
        public int PlanId { get; set; }
        [Required]
        public int DurationTypeId { get; set; }
        [Required]
        public int DurationMonths { get; set; }
        [Required]
        public string PlanName { get; set; }
        [Required]
        public string DurationName { get; set; }
        [Required]
        public double DiscountPercent { get; set; }
    }
}

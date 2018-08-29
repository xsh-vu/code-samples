using System;
using System.ComponentModel.DataAnnotations;

namespace Models.Requests
{
    public class TransactionsAddRequest
    {
        [Required]
        public int UserBaseId { get; set; }
        [Required]
        public int PlanId { get; set; }
        [Required]
        public int DurationTypeId { get; set; }

        public double DiscountPercent { get; set; }
        [Required]
        public string ChargeId { get; set; }
        [Required]
        public double AmountPaid { get; set; }
        [Required]
        public string Currency { get; set; }
        [Required]
        public string NetworkStatus { get; set; }
        [Required]
        public string Type { get; set; }
        [Required]
        public DateTime CreatedDate { get; set; }
        [Required]
        public bool isPaid { get; set; }

        public string CardId { get; set; }

        public int ExpMonth { get; set; }

        public int ExpYear { get; set; }

        public string CardLast4 { get; set; }

    }
}
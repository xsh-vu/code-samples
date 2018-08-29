using Data.Structured;
using System;
using System.ComponentModel.DataAnnotations;


namespace Models.Domain
{
    public class ChargeExistingUser
    {
        [Required]
        [GenericTable(System.Data.SqlDbType.NVarChar, "CustomerId", 50)]
        public string CustomerId { get; set; }

        [Required]
        [GenericTable(System.Data.SqlDbType.NVarChar, "CustomerName", 50)]
        public string CustomerName { get; set; }

        [Required]
        [GenericTable(System.Data.SqlDbType.NVarChar, "CustomerEmail", 50)]
        public string CustomerEmail { get; set; }

        [Required]
        [GenericTable(System.Data.SqlDbType.NVarChar, "PlanName", 50)]
        public string PlanName { get; set; }

        [Required]
        [GenericTable(System.Data.SqlDbType.NVarChar, "DurationName", 50)]
        public string DurationName { get; set; }

        [Required]
        [GenericTable(System.Data.SqlDbType.Decimal, "DiscountPercent")]
        public double DiscountPercent { get; set; }

        [Required]
        [GenericTable(System.Data.SqlDbType.Decimal, "Price")]
        public double Price { get; set; }

        [Required]
        [GenericTable(System.Data.SqlDbType.NVarChar, "Currency", 10)]
        public string Currency { get; set; }

        [Required]
        [GenericTable(System.Data.SqlDbType.DateTime, "StartDate")]
        public DateTime StartDate { get; set; }

        [Required]
        [GenericTable(System.Data.SqlDbType.DateTime, "NextBillingDate")]
        public DateTime NextBillingDate { get; set; }

        [Required]
        [GenericTable(System.Data.SqlDbType.Int, "UserBaseId")]
        public int UserBaseId { get; set; }

        [Required]
        [GenericTable(System.Data.SqlDbType.Int, "UserProfileId")]
        public int UserProfileId { get; set; }

        [Required]
        [GenericTable(System.Data.SqlDbType.Int, "PlanId")]
        public int PlanId { get; set; }

        [Required]
        [GenericTable(System.Data.SqlDbType.Int, "DurationTypeId")]
        public int DurationTypeId { get; set; }

        [Required]
        [GenericTable(System.Data.SqlDbType.Int, "DurationMonths")]
        public int DurationMonths { get; set; }

        [Required]
        public bool IsActive { get; set; }
    }
}

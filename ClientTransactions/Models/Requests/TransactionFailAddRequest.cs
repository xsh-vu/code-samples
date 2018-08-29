using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Requests
{
    public class TransactionFailAddRequest
    {
        public int UserBaseId { get; set; }
        public string CustomerId { get; set; }
        public double Amount { get; set; }
        public string Currency { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int PlanId { get; set; }
        public int DurationTypeId { get; set; }
        public double DiscountPercent { get; set; }
        public string ErrorMessage { get; set; }
    }
}

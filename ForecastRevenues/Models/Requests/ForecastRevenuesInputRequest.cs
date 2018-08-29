using System;
using System.ComponentModel.DataAnnotations;

namespace Models.Requests.Forecasts
{
    public class ForecastRevenuesInputRequest
    {
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        [Required]
        public int PlanId { get; set; }
        [Required]
        public int DurationId { get; set; }
    }
}
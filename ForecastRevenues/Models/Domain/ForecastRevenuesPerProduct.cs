namespace Eleveight.Models.Domain.Forecasts
{
    public class ForecastRevenuesPerProduct
    {
        public int ThisYear { get; set; }
        public string ThisMonth { get; set; }
        public int PlanId { get; set; }
        public int DurationTypeId { get; set; }
        public string PlanName { get; set; }
        public string DurationName { get; set; }
        public double NumAvg { get; set; }
        public double Forecast { get; set; }
    }
}
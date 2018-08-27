using System.Collections.Generic;

namespace Eleveight.Models.Domain.Forecasts
{
    public class ForecastRevenues
    {
        public List<ForecastRevenuesPerProduct> ForecastRevenuesPerProductList { get; set; }
        public List<TotalForecastRevenue> TotalForecastRevenuesList { get; set; }
    }
}
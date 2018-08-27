using Eleveight.Models.Domain.Forecasts;
using Eleveight.Models.Requests.Forecasts;
namespace Eleveight.Services.Forecasts
{
    public interface IForecastService
    {
        ForecastRevenues ReadAllMonthlyForecasts(ForecastRevenuesInputRequest model);
    }
}

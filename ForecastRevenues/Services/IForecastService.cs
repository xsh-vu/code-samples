using Models.Domain.Forecasts;
using Models.Requests.Forecasts;
namespace Services.Forecasts
{
    public interface IForecastService
    {
        ForecastRevenues ReadAllMonthlyForecasts(ForecastRevenuesInputRequest model);
    }
}

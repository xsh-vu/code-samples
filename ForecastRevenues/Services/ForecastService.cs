using Models.Domain.Forecasts;
using Models.Requests.Forecasts;
using Services.Tools;
using System.Data.SqlClient;
using System.Data;
using System.Collections.Generic;

namespace Services.Forecasts
{
    public class ForecastService : BaseService, IForecastService
    {
        //Forecast Revenues per MONTH in specified date range
        public ForecastRevenues ReadAllMonthlyForecasts(ForecastRevenuesInputRequest model)
        {
            ForecastRevenues results = new ForecastRevenues();
            List<ForecastRevenuesPerProduct> forecastProdList = new List<ForecastRevenuesPerProduct>();
            List<TotalForecastRevenue> totForecastList = new List<TotalForecastRevenue>();
            DataProvider.ExecuteCmd("dbo.PlanTransactions_SelectForecastRevenues",
                inputParamMapper: (SqlParameterCollection inputs) =>
                {
                    inputs.AddWithValue("@StartDate", model.StartDate);
                    inputs.AddWithValue("@EndDate", model.EndDate);
                    inputs.AddWithValue("@PlanId", model.PlanId);
                    inputs.AddWithValue("@DurationId", model.DurationId);
                },
                singleRecordMapper: (IDataReader reader, short resultSet) =>
                {
                    if (resultSet == 0)
                    {
                        forecastProdList.Add(DataMapper<ForecastRevenuesPerProduct>.Instance.MapToObject(reader));
                    }
                    else if (resultSet == 1)
                    {
                        totForecastList.Add(DataMapper<TotalForecastRevenue>.Instance.MapToObject(reader));
                    }
                });
            results.ForecastRevenuesPerProductList = forecastProdList;
            results.TotalForecastRevenuesList = totForecastList;
            return results;
        }
    }
}

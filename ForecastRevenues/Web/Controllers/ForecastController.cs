using Models.Responses;
using Models.Domain.Forecasts;
using Models.Requests.Forecasts;
using Services.Forecasts;
using System;
using System.Web.Http;

namespace Web.Controllers.Api.Forecasts
{
    [AllowAnonymous]
    [RoutePrefix("api/forecasts/forecastrevenues")]
    public class ForecastController : ApiController
    {
        IForecastService _forecastService;

        public ForecastController(IForecastService forecastService)
        {
            _forecastService = forecastService;
        }

        [Route(), HttpPost]
        public IHttpActionResult GetAllMonthlyForecasts(ForecastRevenuesInputRequest model)
        {
            try
            {
                ItemResponse<ForecastRevenues> response = new ItemResponse<ForecastRevenues>
                {
                    Item = _forecastService.ReadAllMonthlyForecasts(model)
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
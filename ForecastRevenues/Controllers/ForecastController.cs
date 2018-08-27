using Eleveight.Models.Responses;
using Eleveight.Models.Domain.Forecasts;
using Eleveight.Models.Requests.Forecasts;
using Eleveight.Services.Forecasts;
using System;
using System.Web.Http;

namespace Eleveight.Web.Controllers.Api.Forecasts
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
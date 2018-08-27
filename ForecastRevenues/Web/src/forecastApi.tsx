import ApiExecute from '../common/apiExecute';
const baseUrl = "/";

const GetAllMonthlyForecasts = (model) => {
    return ApiExecute(`${baseUrl}api/forecasts/forecastrevenues`, "POST", model);
};

export const ForecastApi = {
    GetAllMonthlyForecasts
};
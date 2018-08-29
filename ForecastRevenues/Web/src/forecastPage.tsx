import * as React from 'react';
import * as moment from 'moment';
import { Button, DropDownList, Input } from '../common/form';
import { Line } from 'react-chartjs-2';
import { IChart } from '../common/charts/IChart';
import { CSVLink, CSVDownload } from 'react-csv';
import { PricingPlansApi } from './pricingPlansApi';
import { DurationTypesApi } from './durationTypesApi';
import { ForecastApi } from './forecastApi';


interface IForecastState {
    chartData: IChart,
    forecastRequest: {
        startDate: Date,
        endDate: Date,
        planId: number,
        durationId: number
    },
    displayed_endDate: Date,
    displayed_startDate: string,
    resetChart: boolean,
    showChart: boolean,
    showInfographic: boolean,
    titleText: string,
    ddlPlanOptions: any[],
    ddlDurationOptions: any[],
    revenue: string,
    avgMonthlyRev: string,
    csvData: string
}

class ForecastPage extends React.Component<any, IForecastState>{
    constructor(props) {
        super(props);
        this.state = {
            chartData: {
                labels: [],
                datasets: [{
                    label: '',
                    data: [],
                    backgroundColor: []
                }]
            },
            forecastRequest: {
                startDate: undefined,
                endDate: undefined,
                planId: 0,
                durationId: 0
            },
            displayed_endDate: new Date('1000-01-01'),
            displayed_startDate: '',
            resetChart: false,
            showChart: false,
            showInfographic: false,
            titleText: '',
            ddlPlanOptions: [
                { value: "", text: "-- Select a plan --" }
            ],
            ddlDurationOptions: [
                { value: "", text: "-- Select a duration --" }
            ],
            revenue: '',
            avgMonthlyRev: '',
            csvData: ''
        }
    }

    componentDidMount() {
        //Populate plan and duration dropdowns
        this.getPlans();
        this.getDurations();

        let monthYr = moment().format('YYYY-MM');
        let str_startDate = monthYr.toString().concat("01"); //date format: YYYY-MM-01
        let startDate = (moment(str_startDate, "YYYY-MM-DD")).toDate();
        let displayed_startDate = moment().format('MMMM YYYY');

        this.setState({
            ...this.state,
            forecastRequest: {
                ...this.state.forecastRequest,
                startDate: startDate
            },
            displayed_startDate: displayed_startDate
        })
    }

    onDateChange = (fieldName, fieldValue) => {

        let str_endDate = fieldValue.toString().concat("01"); //date format: YYYY-MM-01
        let endDate = moment(str_endDate, 'YYYY-MM-DD').toDate(); //string => date
        let nextState = {
            ...this.state,
            displayed_endDate: fieldValue,
            resetChart: true,
            forecastRequest: {
                ...this.state.forecastRequest,
                endDate: endDate
            }
        }

        this.setState(nextState);
    }

    onChange = (fieldName, fieldValue) => {
        let nextState = {
            ...this.state,
            forecastRequest: {
                ...this.state.forecastRequest,
                [fieldName]: fieldValue
            }
        }
        this.setState(nextState);
    }

    getPlans = () => {
        PricingPlansApi.GetPricingPlans()
            .then(response => {
                let ddl = this.state.ddlPlanOptions.concat(response.items.map(item => {
                    return { value: item.id, text: item.planName }
                }));
                this.setState({
                    ...this.state,
                    ddlPlanOptions: ddl,

                })
            })
            .catch(err => console.log("Error:", err));
    }

    getDurations = () => {
        DurationTypesApi.GetDurationTypes()
            .then(response => {
                let ddl = this.state.ddlDurationOptions.concat(response.items.map(item => {
                    return { value: item.id, text: item.durationName }
                }));
                this.setState({
                    ...this.state,
                    ddlDurationOptions: ddl,
                })
            })
            .catch(err => console.log("Error:", err));
    }

    getAllMonthlyForecasts = () => {
        //Get all revenues per month and set bar chart data 
        ForecastApi.GetAllMonthlyForecasts(this.state.forecastRequest)
            .then(response => {

                if (this.state.resetChart == true) {
                    const labelsArr = response.item.totalForecastRevenuesList.map(itm => itm.thisMonth);
                    const dataArr = response.item.totalForecastRevenuesList.map(itm => itm.forecast);

                    let rev = 0;
                    response.item.totalForecastRevenuesList.map(itm => rev += itm.forecast); //calc total revenue by summing forecasts returned
                    let revenue = `$${this.priceWithCommas(parseFloat(rev.toFixed(2)))}`; //string revenue

                    let avgMoRev = rev / (response.item.totalForecastRevenuesList.length); //calc avg monthly rev 
                    let avgMonthlyRev = `$${this.priceWithCommas(parseFloat(avgMoRev.toFixed(2)))}`; //string avgMonthlyRev

                    const titleText = `Forecast: ${(moment(this.state.forecastRequest.startDate, 'YYYY-MM-DD').format('MMM YYYY'))} - ${(moment(this.state.forecastRequest.endDate, 'YYYY-MM-DD').format('MMM YYYY'))}`;
                    let csv = this.json2csv(response.item.totalForecastRevenuesList);

                    this.setState({
                        ...this.state,
                        resetChart: false,
                        titleText: titleText,
                        csvData: csv,
                        revenue: revenue,
                        avgMonthlyRev: avgMonthlyRev,
                        chartData: {
                            ...this.state.chartData,
                            labels: labelsArr,
                            datasets: [{
                                label: 'Total Revenue',
                                backgroundColor: ['rgba(32, 162, 219, 0.3)'], //light blue fill
                                borderColor: 'rgba(0, 153, 153, 0.5)', //blue-green border
                                data: dataArr
                            }],
                            options: {
                                scales: {
                                    xAxes: [{
                                        stacked: true,
                                    }],
                                    yAxes: [{
                                        stacked: true,
                                    }]
                                }
                            }
                        }
                    }, () => {
                        this.addLine(response.item.forecastRevenuesPerProductList, response.item.totalForecastRevenuesList);
                    })
                }

                else {
                    this.addLine(response.item.forecastRevenuesPerProductList, response.item.totalForecastRevenuesList);
                }

            })
            .catch(error => console.log(error))
    }

    priceWithCommas = (price) => {
        let parts = price.toString().split(".");
        parts[0] = parts[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
        return parts.join(".");
    }

    json2csv = (jsonArr) => {

        let csv = 'Month,Year,Total Revenue \r\n';
        jsonArr.map((itm, index) => {
            csv += `${itm.thisMonth},${itm.thisYear},${itm.forecast}\r\n`;
        })
        return csv;
    }

    onClick = () => {
        //Get all revenues per month and set bar chart data 
        this.getAllMonthlyForecasts();
        this.getRandom_rgba(0.5);
        this.setState({
            ...this.state,
            showChart: true,
            showInfographic: true
        })
    }

    addLine = (productRevList, totalRevList) => {
        if (this.state.forecastRequest.planId != 0 && this.state.forecastRequest.durationId != 0
            && productRevList != null) {
            let _datasets = this.state.chartData.datasets;
            let label = `${productRevList[0].planName.split(" ")[0]} x ${productRevList[0].durationName.split(" ")[0]}`;
            const backgroundArr = [];
            const color = this.getRandom_rgba(0.5);
            backgroundArr.push(color.fillColor);
            const borderColor = color.borderColor;
            let dataArr = [];
            let d = {};
            totalRevList.map(itm => {
                d[itm.thisMonth] = 0;
            })

            productRevList.map(forecast => {
                d[forecast.thisMonth] = forecast.forecast;
            })

            for (const prop in d) {
                dataArr.push(d[prop])
            }
            _datasets.push({
                label: label,
                backgroundColor: backgroundArr,
                borderColor: borderColor,

                data: dataArr
            })

            this.setState({
                ...this.state,
                chartData: {
                    ...this.state.chartData,
                    //labels: labelsArr,
                    datasets: _datasets
                },

            })
        }
    }

    getRandom_rgba = (alpha) => {
        let color = { fillColor: '', borderColor: '' };
        let o = Math.round, r = Math.random, s = 255;
        let fillColor = 'rgba(' + o(r() * s) + ',' + o(r() * s) + ',' + o(r() * s) + ',' + alpha + ')';
        let borderColor = fillColor.slice(0, -4).concat(`${alpha + 0.2})`);
        color.fillColor = fillColor;
        color.borderColor = borderColor;

        return (color);
    }


    render() {
        return (
            <React.Fragment>
                <div>
                    <div className="card-header mb-4">
                        <div>
                            <h2 className="mb-1">Forecasts</h2>
                        </div>
                    </div>


                    <div className="row">
                        <div className="col-1"></div>
                        <div className="col-4">
                            <div className="card mp-3 mb-4">
                                <div style={{ padding: '1rem' }} className="card-body">
                                    <div className="form-group row">
                                        <div className="col">
                                            <label htmlFor="startDate" className="col-form-label">Current Month</label>

                                            <p style={{ padding: '.5rem .875rem', paddingLeft: 0 }}>{this.state.displayed_startDate}</p>

                                            <DropDownList
                                                onChange={this.onChange}
                                                options={this.state.ddlPlanOptions}
                                                label="Plans"
                                                labelStyle={{ 'marginBottom': 0, 'paddingBottom': 0, 'lineHeight': 1.47, 'fontWeight': 500, 'fontSize': '.83125rem' }}
                                                selectedValue={this.state.forecastRequest.planId}
                                                name="planId"
                                            />

                                        </div>
                                        <div className="col">
                                            <label htmlFor="endDate" className="col-form-label">End Month</label>

                                            <Input
                                                type="month"
                                                name="endDate"
                                                value={this.state.displayed_endDate}
                                                onChange={this.onDateChange}
                                                className="form-control"
                                            />

                                            <DropDownList
                                                onChange={this.onChange}
                                                options={this.state.ddlDurationOptions}
                                                label="Durations"
                                                labelStyle={{ 'marginBottom': 0, 'paddingBottom': 0, 'lineHeight': 1.47, 'fontWeight': 500, 'fontSize': '.83125rem' }}
                                                selectedValue={this.state.forecastRequest.durationId}
                                                name="durationId"
                                            />

                                        </div>
                                    </div>

                                    <Button
                                        label="Add Forecast"
                                        className="btn btn-primary waves-effect pull-right"
                                        onClick={this.onClick}
                                        disabled={this.state.forecastRequest.endDate ? false : true}
                                    />
                                </div>
                            </div>
                        </div>
                        <br />
                        {this.state.showInfographic &&

                            <div className="animated bounce col-6">
                                <div className="card mp-2">

                                    <div style={{ backgroundImage: 'linear-gradient(141deg, #9fb8ad 0%, #1fc8db 51%, #2cb5e8 75%)', opacity: 0.95, height: '236.45px' }} className="card-body">
                                        <div className="row">
                                            <div className="col text-center grow">
                                                <img src="https://image.ibb.co/jdf3Qz/noun_Coins_61431.png" height="100" width="100" />
                                                <h3 style={{ color: 'rgb(168, 255, 238)' }}>Forecast Period Total</h3>
                                                <h2 style={{ color: '#FFFFFF' }}>{this.state.revenue}</h2>
                                            </div>
                                            <div className="col text-center grow">
                                                <img style={{ marginTop: '10px' }} src="https://image.ibb.co/b5XpyK/increased_revenue.png" height="65" width="65" />
                                                <h3 style={{ color: 'rgb(168, 255, 238)', paddingTop: '25px' }}>Avg Forecast/Mo</h3>
                                                <h2 style={{ color: '#FFFFFF' }}>{this.state.avgMonthlyRev}</h2>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>

                        }
                        <div className="col-1"></div>
                    </div>


                    <div className="row">
                        <br />
                        <div style={{ float: 'none', margin: '0 auto' }} className="col-10">
                            {this.state.showChart &&
                                <div className="card mp-2">
                                    <div className="row modal-header">
                                        <div className="col pull-left">
                                            <CSVLink className="btn ion ion-md-download btn-outline-secondary borderless mt-2" data={this.state.csvData} filename={"monthlyForecasts.csv"}></CSVLink>
                                        </div>
                                        <br />
                                        <div>
                                            <strong>{this.state.titleText}</strong>
                                        </div>
                                    </div>
                                    <div className="card-body">
                                        <Line
                                            data={this.state.chartData}
                                        />
                                        <br />
                                    </div>

                                </div>

                            }

                        </div>
                    </div>


                </div>
            </React.Fragment>
        )
    }
}

export default ForecastPage;
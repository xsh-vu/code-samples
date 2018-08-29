import * as React from 'react';
import {
    BrowserRouter as Router,
    Route,
    RouteComponentProps
} from 'react-router-dom';
import {
    CardElement,
    CardNumberElement,
    CardExpiryElement,
    CardCVCElement,
    PostalCodeElement,
    PaymentRequestButtonElement,
    IbanElement,
    IdealBankElement,
    StripeProvider,
    Elements,
    injectStripe
} from 'react-stripe-elements';
import queryString from 'query-string';
import Checkout from './Checkout';
import { PaymentApi } from './PaymentApi';
import CardholderInfoForm from './CardholderInfoForm';

interface IPaymentPage {

    chargeRequest: {
        amount: number,
        currency: string,
        name: string,
        email: string,
        sourceTokenOrExistingSourceId: string,
        planId: number,
        durationTypeId: number,
        durationMonths: number,
        planName: string,
        durationName: string,
        discountPercent: number
    }
}

class PaymentPage extends React.Component<{} & RouteComponentProps<{}>, any> {

    constructor(props) {
        super(props);
        this.state = {
            chargeRequest: {
                amount: 0,
                currency: '',
                name: '',
                email: '',
                sourceTokenOrExistingSourceId: '',
                planId: 0,
                durationTypeId: 0,
                planName: '',
                durationName: '',
                durationMonths: 0,
                discountPercent: 0
            }
        }
    }

    componentDidMount() {
        let values = queryString.parse(this.props.location.search);

        this.setState({
            chargeRequest: {
                ...this.state.chargeRequest,
                amount: values.amount,
                currency: values.currency,
                planId: values.planId,
                durationTypeId: values.durationTypeId,
                planName: values.plan,
                durationName: values.duration,
                durationMonths: values.durationMonths,
                discountPercent: values.discountPercent
            }
        })
    }

    onChange = (fieldName, fieldValue) => {
        let nextState = {
            ...this.state,
            chargeRequest: {
                ...this.state.chargeRequest,
                [fieldName]: fieldValue
            }
        };
        this.setState(nextState, () => console.log(nextState));
    };

    onPay = (token) => {
        this.setState({
            chargeRequest: {
                ...this.state.chargeRequest,
                sourceTokenOrExistingSourceId: token
            }
        }, () => {
            //console.log("token", this.state.sourceTokenOrExistingSourceId)
            //Charge request
            PaymentApi.postCharge(this.state.chargeRequest)
                .then((res) => {
                    console.log("success", res);
                })
                .catch(err => console.log("ERROR:", err));
        })
    };

    render() {

        return (
            <React.Fragment>
                <StripeProvider apiKey="">
                    <Checkout onPay={this.onPay}
                        onChange={this.onChange}
                        chargeRequest={this.state.chargeRequest}
                    />
                </StripeProvider>
            </React.Fragment>
        );
    }

}

export default PaymentPage;
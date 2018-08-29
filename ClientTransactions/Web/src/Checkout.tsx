import * as React from 'react';
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

import { CardForm } from './PaymentCardForm';
import CardholderInfoForm from './CardholderInfoForm';

interface CheckoutProps {
    onPay: (token: string) => void;
    onChange: (fieldName: string, fieldValue: string) => void;
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

class Checkout extends React.Component<{} & CheckoutProps, { elementFontSize: string }> {
    constructor(props) {
        super(props);
        this.state = {
            elementFontSize: window.innerWidth < 450 ? '14px' : '18px',
        };
        window.addEventListener('resize', () => {
            if (window.innerWidth < 450 && this.state.elementFontSize !== '14px') {
                this.setState({ elementFontSize: '14px' });
            } else if (
                window.innerWidth >= 450 &&
                this.state.elementFontSize !== '18px'
            ) {
                this.setState({ elementFontSize: '18px' });
            }
        });
    }

    render() {
        const { elementFontSize } = this.state;
        return (
            <div className="Checkout">
                <div className="row">
                    <div className="col">
                        <div className="card-header mb-4">
                            <div>
                                <h2 className="mb-1">Payment Details</h2>
                            </div>
                        </div>
                        <div className="StripeElement">
                            <CardholderInfoForm
                                onChange={this.props.onChange}
                                chargeRequest={this.props.chargeRequest}
                            />
                        </div>

                        <Elements>
                            <CardForm
                                onPay={this.props.onPay}
                                fontSize={elementFontSize}
                            />
                        </Elements>

                        <br />

                    </div>
                    <div className="orderSummary">
                        <div className="card-header mb-4">
                            <div>
                                <h2 className="mb-1">Order Summary:</h2>
                            </div>
                        </div>

                        <div className="card">
                            <div style={{ padding: '0rem' }} className="card-body">
                                <div className="container px-3 my-5 clearfix">
                                    <div className="table-responsive">
                                        <table className="table table-bordered m-0">
                                            <thead>
                                                <tr>
                                                    <th className="text-center py-3 px-4" style={{ width: "400px" }}>Plan Name &amp; Details</th>
                                                    <th className="text-right py-3 px-4" style={{ width: "100px" }}>Price</th>
                                                    <th className="text-center py-3 px-4" style={{ width: "120px" }}>Duration</th>
                                                    <th className="text-right py-3 px-4" style={{ width: "100px" }}>Total</th>

                                                </tr>
                                            </thead>
                                            <tbody>
                                                <tr>
                                                    <td className="p-4">
                                                        <div className="media align-items-center">
                                                            <img src="https://i.pinimg.com/originals/e5/b5/bb/e5b5bb28a0e3a4d7a0df8d7e3a4db083.png" className="d-block ui-w-40 ui-bordered mr-4" alt="" />
                                                            <div className="media-body">
                                                                <a href="#" className="d-block text-dark">{this.props.chargeRequest.planName}</a>

                                                                <small>
                                                                    <span className="text-muted">Users: </span>1000&nbsp;
                                                <span className="text-muted">Projects: </span>1000&nbsp;
                                                <span className="text-muted">Storage: </span>100
                                                                </small>
                                                            </div>
                                                        </div>
                                                    </td>
                                                    <td className="text-right font-weight-semibold align-middle p-4">${(this.props.chargeRequest.amount / this.props.chargeRequest.durationMonths)}</td>
                                                    <td className="text-right font-weight-semibold align-middle p-4">{this.props.chargeRequest.durationMonths} mo</td>
                                                    <td className="text-right font-weight-semibold align-middle p-4">${this.props.chargeRequest.amount}</td>

                                                </tr>
                                            </tbody>
                                        </table>
                                    </div>
                                    <div className="d-flex flex-wrap justify-content-between align-items-center pb-4">
                                        <div className="mt-4">
                                            <label className="text-muted font-weight-normal">Promocode</label>
                                            <input type="text" placeholder="ABC" className="form-control" />
                                        </div>
                                        <div className="d-flex">
                                            <div className="text-right mt-4 mr-5">
                                                <label className="text-muted font-weight-normal m-0">Discount</label>
                                                <div className="text-large"><strong>${(this.props.chargeRequest.amount / this.props.chargeRequest.durationMonths)}</strong></div>
                                            </div>
                                            <div className="text-right mt-4">
                                                <label className="text-muted font-weight-normal m-0">Total price</label>
                                                <div className="text-large"><strong>${this.props.chargeRequest.amount}</strong></div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>


                </div>
            </div>
        );
    }
}

export default Checkout;
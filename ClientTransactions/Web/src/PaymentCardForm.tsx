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

const handleBlur = () => {
    console.log('[blur]');
};
const handleChange = (change) => {
    console.log('[change]', change);
};
const handleClick = () => {
    console.log('[click]');
};
const handleFocus = () => {
    console.log('[focus]');
};
const handleReady = () => {
    console.log('[ready]');
};

const createOptions = (fontSize: string) => {
    return {
        style: {
            base: {
                fontSize,
                color: '#424770',
                letterSpacing: '0.025em',
                fontFamily: 'Source Code Pro, monospace',
                '::placeholder': {
                    color: '#aab7c4',
                },
                //padding,
            },
            invalid: {
                color: '#9e2146',
            },
        },
    };
};

const buttonStyle: React.CSSProperties = {
    whiteSpace: 'nowrap',
    border: '0',
    outline: '0',
    display: 'inline - block',
    height: '40px',
    lineHeight: '40px',
    padding: '0 14px',
    boxShadow: '0 4px 6px rgba(50, 50, 93, .11), 0 1px 3px rgba(0, 0, 0, .08)',
    color: '#fff',
    borderRadius: '4px',
    fontSize: '15px',
    fontWeight: 600,
    textTransform: 'uppercase',
    letterSpacing: '0.025em',
    backgroundColor: '#6772e5',
    textDecoration: 'none',
    //- webkit - transition: all 150ms ease;
    transition: 'all 150ms ease',
    marginTop: '10px'
};

interface _CardFormProps {
    stripe: any,
    onPay: (token: string) => void;
}


class _CardForm extends React.Component<_CardFormProps & { fontSize: string }> {
    handleSubmit = (e) => {
        e.preventDefault();
        if (this.props.stripe) {
            this.props.stripe
                .createToken()
                .then((payload) => {
                    console.log('[token]', payload)
                    this.props.onPay(payload.token.id);
                });
        } else {
            console.log("Stripe.js hasn't loaded yet.");
        }
    };
    render() {
        return (
            <form onSubmit={this.handleSubmit} >
                <label style={{ width: '100%' }}>
                    {//Card details
                    }
                    <CardElement
                        onBlur={handleBlur}
                        onChange={handleChange}
                        onFocus={handleFocus}
                        onReady={handleReady}
                        {...createOptions(this.props.fontSize)}
                    />
                </label>
                <button style={buttonStyle}>Pay</button>
            </form>
        );
    }
}
export const CardForm = injectStripe(_CardForm);



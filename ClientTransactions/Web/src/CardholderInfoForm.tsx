import * as React from 'react';
import { Input } from '../common/form';


interface ICardholderInfoProps {
    onChange: (fieldName: string, fieldValue: string) => void;
    chargeRequest: {
        amount: number,
        currency: string,
        name: string,
        email: string,
        sourceTokenOrExistingSourceId: string
    }
}

class CardholderInfoForm extends React.Component<ICardholderInfoProps> {

    render() {
        return (
            <React.Fragment>
                <Input
                    type="text"
                    label="Name"
                    name="name"
                    placeholder="Kim Taehyung"
                    value={this.props.chargeRequest.name}
                    onChange={this.props.onChange}
                //error={props.error.email}
                />
                <Input
                    type="text"
                    label="Email"
                    name="email"
                    placeholder="kimtaehyung@bts.com"
                    value={this.props.chargeRequest.email}
                    onChange={this.props.onChange}
                //error={props.error.email}
                />

            </React.Fragment>
        );
    }
}
export default CardholderInfoForm;



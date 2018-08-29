import * as React from 'react';
import { ForgotPasswordLayout } from './forgotPasswordLayout';
import { ForgotPasswordApi } from './forgotPasswordApi';
import { ITestCase, formatTestCase, validateFields } from '../common/DynamicRuleValidation';

interface IForgotPasswordState {
    email: {
        email: string
    }
    regEmailMsg: string
    GUID: string
    error: {
        email: string
    }
    isFormValid: boolean
}

class ForgotPasswordPage extends React.Component<{}, IForgotPasswordState>{
    constructor(props) {
        super(props);
        this.state = {
            email: {
                email: ""
            },
            regEmailMsg: "",
            GUID: "",
            error: {
                email: ""
            },

            isFormValid: false
        }
    }

    onChange = (fieldName, fieldValue) => {
        let nextState = {
            ...this.state, email: {
                email: fieldValue
            }
        }
        this.setState(nextState, () => {
            this.validateFields(this.state.email, fieldName); //validate password
        });
    }

    // On submit of email address:
    onSubmit = () => {
        ForgotPasswordApi.PostGUID(this.state.email) //get GUID token
            .then((res) => {
                this.setState({
                    ...this.state,
                    GUID: res.item
                })

                if (res.item == "0") { //DB returns "0" if no registered email found in the DB
                    this.setState({
                        ...this.state,
                        regEmailMsg: "No registered account was found with that email."
                    })
                }
                else {
                    this.setState({
                        ...this.state,
                        regEmailMsg: `Reset password link was sent to ${this.state.email.email}`
                    })
                }
            })
            .catch(err => console.log("ERROR:", err));
    }

    //Validate field fn
    validateFields = (form: any, fieldName: string) => {
        let tests: ITestCase[] = new Array<ITestCase>();
        for (let field in form) {
            let rules = {};
            switch (field) {
                case "email":
                    rules = {
                        minLength: 3,
                        maxLength: 50,
                        validEmail: true
                    }
                    break;
                default:
                    break;
            }
            tests.push(formatTestCase(form[field], field, rules, new Array<string>()))
        }
        tests = validateFields(tests);

        let newErrMsgs = { ...this.state.error };
        let currentFieldTest = tests.find(test => test.field == fieldName);
        if (currentFieldTest.errMsg.length > 0 && currentFieldTest.value)
            newErrMsgs = { ...this.state.error, [fieldName]: currentFieldTest.errMsg[0] };
        else newErrMsgs = { ...this.state.error, [fieldName]: "" }
        this.setState({
            ...this.state, isFormValid: tests.every(test => test.errMsg.length == 0), error: newErrMsgs
        })
    }

    render() {
        return (
            <React.Fragment>
                <ForgotPasswordLayout
                    onChange={this.onChange}
                    onSubmit={this.onSubmit}
                    email={this.state.email.email}
                    error={this.state.error}
                    disabled={!this.state.isFormValid}
                    regEmailMsg={this.state.regEmailMsg}
                />
            </React.Fragment>
        );
    }
}

export default ForgotPasswordPage;
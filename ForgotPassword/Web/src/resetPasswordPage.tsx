import * as React from "react";
import {
    BrowserRouter as Router,
    Route,
    RouteComponentProps
} from 'react-router-dom'
import { ResetPasswordLayout } from "./resetPasswordLayout";
import { ForgotPasswordApi } from "./forgotPasswordApi";
import { ITestCase, formatTestCase, validateFields } from "../common/DynamicRuleValidation";

interface IResetPasswordState {
    passwords: {
        newPassword: string,
        confirmPassword: string,
    }

    UpdatePasswordModel: {
        Password: string,
        GUID: string
    }

    error: {
        newPassword: string,
        confirmPassword: string
    }

    isFormValid: boolean
}

class ResetPasswordPage extends React.Component<{} & RouteComponentProps<{}>, IResetPasswordState>{
    constructor(props) {
        super(props);
        this.state = {
            passwords: {
                newPassword: "",
                confirmPassword: ""
            },

            UpdatePasswordModel: {
                Password: "",
                GUID: ""
            },

            error: {
                newPassword: "",
                confirmPassword: ""
            },

            isFormValid: false
        }
    }

    componentDidMount() {
        const urlGUID = this.props.match.params["GUID"];
        //console.log(urlGUID);
        this.setState({
            ...this.state,
            UpdatePasswordModel: {
                ...this.state.UpdatePasswordModel,
                GUID: urlGUID
            }
        })
    }

    onChange = (fieldName, fieldValue) => {
        let nextState = {
            ...this.state,
            passwords: {
                ...this.state.passwords,
                [fieldName]: fieldValue
            }
        }
        this.setState(nextState, () => {
            //console.log(this.state);
            this.validateFields(this.state.passwords, fieldName); //validate password
        });
    }

    onSubmit = () => {

        ForgotPasswordApi.PutPassword(this.state.UpdatePasswordModel)
            .then((res) => {
                //console.log(res)
                this.props.history.push("/");
            })
            .catch(err => console.log("ERROR:", err));
    }

    //Validate fields function
    validateFields = (form: any, fieldName: string) => {
        let tests: ITestCase[] = new Array<ITestCase>();
        for (let field in form) {
            let rules = {};
            switch (field) {
                case "newPassword":

                case "confirmPassword":
                    rules = {
                        validPassword: true
                    }
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
            ...this.state, isFormValid: tests.every(test => test.errMsg.length == 0) &&
                this.state.passwords.confirmPassword == this.state.passwords.newPassword, error: newErrMsgs,
            UpdatePasswordModel: {
                ...this.state.UpdatePasswordModel,
                Password: this.state.passwords.newPassword
            }
        })
    }

    render() {
        return (
            <React.Fragment>
                <ResetPasswordLayout
                    onChange={this.onChange}
                    onSubmit={this.onSubmit}
                    newPassword={this.state.passwords.newPassword}
                    confirmPassword={this.state.passwords.confirmPassword}
                    error={this.state.error}
                    disabled={!this.state.isFormValid}
                />
            </React.Fragment>
        );
    }
}

export default ResetPasswordPage;
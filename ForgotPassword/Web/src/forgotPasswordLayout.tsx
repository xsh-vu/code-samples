import * as React from 'react';
import { Input, Button } from '../common/form';

interface IForgotPasswordProps {
    onChange: (fieldName: string, fieldValue: string) => void;
    onSubmit: (e) => void;
    email: string
    error: any
    disabled: boolean
    regEmailMsg: string
}

export const ForgotPasswordLayout = (props: IForgotPasswordProps) => (
    <React.Fragment>
        <div className="authentication-wrapper authentication-3">
            <div className="authentication-inner row" style={{ height: "100vh", width: "100%" }}>
                <div className="d-none d-lg-flex col-lg-8 align-items-center ui-bg-cover ui-bg-overlay-container p-5" style={{ backgroundImage: `url("Content/assets/img/bg/21.jpg")` }}>
                    <div className="ui-bg-overlay bg-dark opacity-50"></div>

                    <div className="w-100 text-white px-5">
                        <h1 className="display-2 font-weight-bolder mb-4">WELCOME TO
								<br />ELEVEIGHT</h1>
                        <div className="text-large font-weight-light">
                            Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vestibulum vehicula ex eu gravida faucibus. Suspendisse viverra pharetra purus. Proin fringilla ac lorem at sagittis. Proin tincidunt dui et nunc ultricies dignissim.
							</div>
                    </div>
                </div>

                <div className="d-flex col-lg-4 align-items-center bg-white p-5">
                    <div className="d-flex col-sm-7 col-md-5 col-lg-12 px-0 px-xl-4 mx-auto">
                        <div className="w-100">

                            <div className="d-flex justify-content-center align-items-center">
                                <div className="ui-w-60">
                                    <img src="https://trello-attachments.s3.amazonaws.com/5b2d69a9d38a230279394f76/5b57b05e904c56bbb72e723c/c1ab727673aa1226ba4d471ba58bbf29/eleveight.png" />
                                </div>
                            </div>

                            <h4 className="text-center text-lighter font-weight-normal mt-5 mb-0">Forgot Password</h4>

                            <form>
                                <Input
                                    type="text"
                                    label="Email"
                                    name="email"
                                    placeholder="Enter your email address"
                                    value={props.email}
                                    onChange={props.onChange}
                                    error={props.error.email}
                                />

                                <p> {props.regEmailMsg} </p>

                                <button className="btn btn-primary btn-block waves-effect" data-toggle="collapse" disabled={props.disabled} onClick={props.onSubmit}> Send password reset email</button>
                            </form>

                        </div>
                    </div>
                </div>

            </div>
        </div >

    </React.Fragment>
)


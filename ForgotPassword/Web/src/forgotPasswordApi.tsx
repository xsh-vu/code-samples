import ApiExecute from '../common/apiExecute';

const baseUrl = "/"

const PostGUID = (model) => {
    return ApiExecute(`${baseUrl}api/app/apptokens/forgotpassword`, "POST", model);
}

const PutPassword = (model) => {
    return ApiExecute(`${baseUrl}api/app/apptokens/${model.GUID}`, "PUT", model);
}

export const ForgotPasswordApi = {
    PostGUID,
    PutPassword
}
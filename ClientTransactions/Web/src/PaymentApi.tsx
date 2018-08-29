import apiExecute from '../common/apiExecute';

const baseUrl = '/';

const postCharge = (model) => {
    return apiExecute(`${baseUrl}api/charges`, 'POST', model)
}

const getAllSubscriptions = () => {
    return apiExecute(`${baseUrl}api/charges`, 'GET', null)
}

export const PaymentApi = {
    postCharge,
    getAllSubscriptions
}
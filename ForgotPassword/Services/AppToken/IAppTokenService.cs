using Models.Domain.App;
using Models.Requests.App;
using Models.Requests.ForgotPassword;
using Models.Requests.ChangePassword;
using System;
using System.Collections.Generic;

namespace Services.AppToken
{
    public interface IAppTokenService
    {
        void Insert(AppTokenAddRequest model);

        List<AppToken> ReadAll();

        void Update(AppTokenUpdateRequest model);

        string InsertGUID(ForgotPasswordAppTokenAddRequest model);

        void UpdatePassword(ForgotPasswordUserBaseUpdateRequest model);

        Boolean ChangePassword(ChangePasswordUserBaseUpdateRequest model);
    }
}
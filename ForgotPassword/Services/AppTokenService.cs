using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Models.Domain.App;
using Models.Domain.User;
using Models.Requests.App;
using Models.Requests.ForgotPassword;
using Models.Requests.ChangePassword;
using Services.Tools;
using Services.Cryptography;


namespace Services.AppToken
{
    public class AppTokenService : BaseService, IAppTokenService
    {
        private const int HASH_ITERATION_COUNT = 1;
        private ICryptographyService _cryptographyService;
        private IUserService _userservice;

        public AppTokenService(IUserService userservice, ICryptographyService cryptographyService)
        {
            _userservice = userservice;
            _cryptographyService = cryptographyService;
        }

        public List<AppToken> ReadAll()
        {
            List<AppToken> list = new List<AppToken>();
            DataProvider.ExecuteCmd("dbo.AppToken_SelectAll",
                inputParamMapper: null,
                singleRecordMapper: (IDataReader reader, short resultSet) =>
                {
                    list.Add(DataMapper<AppToken>.Instance.MapToObject(reader));
                });
            return list;
        }

        public void Insert(AppTokenAddRequest model)
        {
            DataProvider.ExecuteNonQuery("dbo.AppToken_Insert",
                inputParamMapper: (SqlParameterCollection inputs) =>
                {
                    inputs.AddWithValue("@UserBaseId", model.UserBaseId);
                    inputs.AddWithValue("@Token", model.Token);
                    inputs.AddWithValue("@AppTokenTypeId", model.AppTokenTypeId);
                });
        }

        public void Update(AppTokenUpdateRequest model)
        {
            DataProvider.ExecuteNonQuery("dbo.AppToken_UpdateToken",
                inputParamMapper: (SqlParameterCollection inputs) =>
                {
                    inputs.AddWithValue("@UserBaseId", model.UserBaseId);
                    inputs.AddWithValue("@Token", model.Token);
                    inputs.AddWithValue("@AppTokenTypeId", model.AppTokenTypeId);
                });
        }

        public string InsertGUID(ForgotPasswordAppTokenAddRequest model)
        {
            string returnValue = "";

            DataProvider.ExecuteNonQuery("dbo.AppToken_InsertByUserBaseEmail",
               inputParamMapper: (SqlParameterCollection inputs) =>
               {
                   inputs.AddWithValue("@Email", model.Email);
                   SqlParameter guidIdOut = new SqlParameter("@GUID", SqlDbType.NVarChar, 128);
                   guidIdOut.Direction = ParameterDirection.Output;
                   inputs.Add(guidIdOut);
               },
                returnParameters: (SqlParameterCollection inputs) =>
                {
                    returnValue = inputs["@GUID"].Value.ToString();
                }
               );
            return returnValue;
        }

        public void UpdatePassword(ForgotPasswordUserBaseUpdateRequest model)
        {
            UserSaltPasswordHash salthash = _userservice.CreateSaltandHash(model.Password);

            DataProvider.ExecuteNonQuery("dbo.UserBase_UpdateByAppTokenUserBaseId",
                inputParamMapper: (SqlParameterCollection inputs) =>
                {
                    inputs.AddWithValue("@GUID", model.GUID);
                    inputs.AddWithValue("@PasswordHash", salthash.passwordHash);
                    inputs.AddWithValue("@Salt", salthash.salt);
                });
        }

        public Boolean ChangePassword(ChangePasswordUserBaseUpdateRequest model)
        {
            Boolean isPasswordChanged;
            UserBase user = new UserBase();
            DataProvider.ExecuteCmd("dbo.UserBase_SelectById",
                inputParamMapper: (SqlParameterCollection inputs) =>
                {
                    inputs.AddWithValue("@id", model.CurrentUserBaseId);
                },
                singleRecordMapper: (IDataReader reader, short resultSet) =>
                {
                    if (resultSet == 0)
                        user = DataMapper<UserBase>.Instance.MapToObject(reader);
                });

            string oldPasswordHash = _cryptographyService.Hash(model.OldPassword, user.Salt, HASH_ITERATION_COUNT);

            if (user.PasswordHash == oldPasswordHash)
            {
                //use user id to get guid
                ForgotPasswordAppTokenAddRequest addRequest = new ForgotPasswordAppTokenAddRequest();
                addRequest.Email = user.Email;
                string currentUserGUID = InsertGUID(addRequest);

                //use guid to update password
                ForgotPasswordUserBaseUpdateRequest updateRequest = new ForgotPasswordUserBaseUpdateRequest();
                updateRequest.Password = model.NewPassword;
                updateRequest.GUID = currentUserGUID;
                UpdatePassword(updateRequest);
                isPasswordChanged = true;
            }
            else
            {
                isPasswordChanged = false;
            }
            return isPasswordChanged;
        }
    }
}
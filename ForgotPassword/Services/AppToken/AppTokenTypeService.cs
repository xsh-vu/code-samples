using Models.Domain.App;
using Models.Requests.App;
using Services.Tools;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Services.App
{
    public class AppTokenTypeService : BaseService, IAppTokenTypeService
    {
        public List<AppTokenType> ReadAll()
        {
            List<AppTokenType> list = new List<AppTokenType>();
            DataProvider.ExecuteCmd("dbo.AppTokenType_SelectAll",
                inputParamMapper: null,
                singleRecordMapper: (IDataReader reader, short resultSet) =>
                {
                    list.Add(DataMapper<AppTokenType>.Instance.MapToObject(reader));
                });
            return list;
        }

        public AppTokenType ReadById(int id)
        {
            AppTokenType appTokenType = new AppTokenType();
            DataProvider.ExecuteCmd("dbo.AppTokenType_SelectById",
                inputParamMapper: (SqlParameterCollection inputs) =>
                {
                    inputs.AddWithValue("@Id", id);
                },
                singleRecordMapper: (IDataReader reader, short resultSet) =>
                {
                    appTokenType = DataMapper<AppTokenType>.Instance.MapToObject(reader);
                });
            return appTokenType;
        }

        public int Insert(AppTokenTypeAddRequest model)
        {
            int returnValue = 0;

            DataProvider.ExecuteNonQuery("dbo.AppTokenType_Insert",
                inputParamMapper: (SqlParameterCollection inputs) =>
                {
                    inputs.AddWithValue("@TypeName", model.TypeName);
                    inputs.AddWithValue("@TypeDescription", model.TypeDescription);

                    SqlParameter idOut = new SqlParameter("@Id", 0);
                    idOut.Direction = ParameterDirection.Output;

                    inputs.Add(idOut);
                },
                returnParameters: (SqlParameterCollection inputs) =>
                {
                    int.TryParse(inputs["@Id"].Value.ToString(), out returnValue);
                });
            return returnValue;
        }

        public void Update(AppTokenTypeUpdateRequest model)
        {
            DataProvider.ExecuteNonQuery("dbo.AppTokenType_Update",
                inputParamMapper: (SqlParameterCollection inputs) =>
                {
                    inputs.AddWithValue("@Id", model.Id);
                    inputs.AddWithValue("@TypeName", model.TypeName);
                    inputs.AddWithValue("@TypeDescription", model.TypeDescription);
                });
        }

        public void Delete(int id)
        {
            DataProvider.ExecuteNonQuery("dbo.AppTokenType_Delete",
                inputParamMapper: (SqlParameterCollection inputs) =>
                {
                    inputs.AddWithValue("@Id", id);
                });
        }
    }
}
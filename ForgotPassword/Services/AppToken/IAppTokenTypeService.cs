using Models.Domain.App;
using Models.Requests.App;
using System.Collections.Generic;

namespace Services.App
{
    public interface IAppTokenTypeService
    {
        List<AppTokenType> ReadAll();

        AppTokenType ReadById(int id);

        int Insert(AppTokenTypeAddRequest model);

        void Update(AppTokenTypeUpdateRequest model);

        void Delete(int id);
    }
}
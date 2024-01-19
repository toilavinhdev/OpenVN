using Microsoft.Extensions.Localization;
using OpenVN.Master.Application;
using OpenVN.Master.Application.Repositories;
using OpenVN.Master.Domain;
using SharedKernel.Auth;
using SharedKernel.Caching;
using SharedKernel.Infrastructures;
using SharedKernel.MySQL;
using SharedKernel.Properties;

namespace OpenVN.Master.Infrastructure.Repositories
{
    public class AppWriteOnlyRepository : BaseWriteOnlyRepository<App>, IAppWriteOnlyRepository
    {
        public AppWriteOnlyRepository(IDbConnection dbConnection, IToken token, ISequenceCaching sequenceCaching, IStringLocalizer<Resources> localizer) : base(dbConnection, token, sequenceCaching, localizer)
        {
        }

        public async Task UpdateFavouriteAsync(long appId, bool isFavourite, CancellationToken cancellationToken)
        {
            var param = new
            {
                v_tenant_id = _token.Context.TenantId,
                v_owner_id = _token.Context.OwnerId,
                v_app_id = appId,
                v_is_favourite = isFavourite,
            };
            await _dbConnection.ExecuteAsync("proc_update_favourite", param, commandType: System.Data.CommandType.StoredProcedure);
        }
    }
}

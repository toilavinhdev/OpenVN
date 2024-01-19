using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using SharedKernel.Auth;
using SharedKernel.Caching;
using SharedKernel.Libraries;
using SharedKernel.MySQL;
using SharedKernel.Properties;

namespace OpenVN.Infrastructure
{
    public class ConfigWriteOnlyRepository : BaseWriteOnlyRepository<UserConfig>, IConfigWriteOnlyRepository
    {
        public ConfigWriteOnlyRepository(
            IDbConnection dbConnection, 
            IToken token, 
            ISequenceCaching sequenceCaching, 
            IStringLocalizer<Resources> localizer
        ) : base(dbConnection, token, sequenceCaching, localizer)
        {
        }

        public async Task<UserConfig> CreateOrUpdateAsync(UserConfig userConfig, CancellationToken cancellationToken)
        {
            userConfig ??= new UserConfig
                           {
                               Json = JsonConvert.SerializeObject(new ConfigValue
                               {
                                   Language = "en-US"
                               }),
                               OwnerId = _token.Context.OwnerId,
                               TenantId = _token.Context.TenantId,
                           };

            var sp = JsonHelper.GetConfiguration("other-stored-procedure.json")["CreateOrUpdateConfig"];
            var param = new
            {
                v_json = userConfig.Json,
                v_owner_id = userConfig.OwnerId,
                v_tenant_id = userConfig.TenantId,
            };
            var result = await (_dbConnection.QuerySingleOrDefaultAsync<UserConfig>(sp, param, commandType: System.Data.CommandType.StoredProcedure));
            var cacheKey = BaseCacheKeys.GetConfigKey(_token.Context.TenantId, _token.Context.OwnerId);

            await _dbConnection.CommitAsync(false, cancellationToken);
            await _sequenceCaching.SetAsync(cacheKey, result, TimeSpan.FromDays(30), cancellationToken: cancellationToken);

            return result;
        }
    }
}

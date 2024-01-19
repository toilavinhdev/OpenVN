using SharedKernel.Auth;
using SharedKernel.Caching;
using SharedKernel.MySQL;

namespace OpenVN.Infrastructure
{
    public class ConfigReadOnlyRepository : BaseReadOnlyRepository<UserConfig>, IConfigReadOnlyRepository
    {
        public ConfigReadOnlyRepository(IDbConnection dbConnection, IToken token, ISequenceCaching sequenceCaching, IServiceProvider provider) : base(dbConnection, token, sequenceCaching, provider)
        {
        }

        public async Task<UserConfig> GetConfigAsync(CancellationToken cancellationToken)
        {
            var cacheKey = BaseCacheKeys.GetConfigKey(_token.Context.TenantId, _token.Context.OwnerId);
            var cacheData = await _sequenceCaching.GetAsync<UserConfig>(cacheKey, cancellationToken: cancellationToken);
            if (cacheData != null)
            {
                return cacheData;
            }

            var cmd = $"SELECT * FROM {new UserConfig().GetTableName()} WHERE OwnerId = @OwnerId AND TenantId = @TenantId AND IsDeleted = 0";
            var param = new
            {
                OwnerId = _token.Context.OwnerId,
                TenantId = _token.Context.TenantId
            };
            return await _dbConnection.QuerySingleOrDefaultAsync<UserConfig>(cmd, param);
        }
    }
}

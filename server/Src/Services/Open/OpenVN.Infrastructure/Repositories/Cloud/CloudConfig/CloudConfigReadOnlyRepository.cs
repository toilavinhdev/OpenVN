using SharedKernel.Auth;
using SharedKernel.Caching;
using SharedKernel.Libraries;
using SharedKernel.MySQL;

namespace OpenVN.Infrastructure
{
    public class CloudConfigReadOnlyRepository : BaseReadOnlyRepository<CloudConfig>, ICloudConfigReadOnlyRepository
    {
        public CloudConfigReadOnlyRepository(IDbConnection dbConnection, IToken token, ISequenceCaching sequenceCaching, IServiceProvider provider) : base(dbConnection, token, sequenceCaching, provider)
        {
        }

        public async Task<CloudConfig> GetCloudConfigAsync(CancellationToken cancellationToken)
        {
            var query = $"SELECT * FROM {new CloudConfig().GetTableName()} WHERE TenantId = {_token.Context.TenantId} AND OwnerId = {_token.Context.OwnerId} AND IsDeleted = 0";
            var result = await _dbConnection.QuerySingleOrDefaultAsync<CloudConfig>(query, cancellationToken);
            if (result == null) // NEED_REFACTOR
            {
                var cmd = @$"INSERT INTO {new CloudConfig().GetTableName()}(Id, TenantId, OwnerId, MaxCapacity, MaxFileSize, CreatedBy)
                             VALUES({AuthUtility.GenerateSnowflakeId()}, {_token.Context.TenantId}, {_token.Context.OwnerId}, 52428800, 52428800, {_token.Context.OwnerId}) ";
                await _dbConnection.ExecuteAsync(cmd, null, autoCommit: true);
            }

            return await _dbConnection.QuerySingleOrDefaultAsync<CloudConfig>(query, cancellationToken);
        }
    }
}

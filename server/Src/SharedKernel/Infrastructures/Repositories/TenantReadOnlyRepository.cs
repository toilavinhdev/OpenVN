using SharedKernel.Application;
using SharedKernel.Auth;
using SharedKernel.Caching;
using SharedKernel.Domain;
using SharedKernel.MySQL;

namespace SharedKernel.Infrastructures
{
    public class TenantReadOnlyRepository : ITenantReadOnlyRepository
    {
        private readonly IDbConnection _dbConnection;
        private readonly ISequenceCaching _sequenceCaching;

        public TenantReadOnlyRepository(
            IDbConnection dbConnection,
            ISequenceCaching sequenceCaching
        )
        {
            _dbConnection = dbConnection;
            _sequenceCaching = sequenceCaching;
        }

        public async Task<IEnumerable<TResult>> GetAllAsync<TResult>(CancellationToken cancellationToken)
        {
            var key = BaseCacheKeys.GetSystemFullRecordsKey(new Tenant().GetTableName());
            var data = await _sequenceCaching.GetAsync<List<TResult>>(key, cancellationToken: cancellationToken);
            if (data != null)
            {
                return data;
            }

            var cmd = $"SELECT * FROM {new Tenant().GetTableName()} WHERE IsDeleted = 0";
            var result = await _dbConnection.QueryAsync<TResult>(cmd);
            if (result.Any())
            {
                await _sequenceCaching.SetAsync(key, result, TimeSpan.FromHours(12), cancellationToken: cancellationToken);
            }
            return result;
        }
    }
}

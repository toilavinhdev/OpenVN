using SharedKernel.Auth;
using SharedKernel.Caching;
using SharedKernel.MySQL;

namespace OpenVN.Infrastructure
{
    public class NoteCategoryReadOnlyRepository : BaseReadOnlyRepository<NoteCategory>, INoteCategoryReadOnlyRepository
    {
        public NoteCategoryReadOnlyRepository(IDbConnection dbConnection, IToken token, ISequenceCaching sequenceCaching, IServiceProvider provider) : base(dbConnection, token, sequenceCaching, provider)
        {
        }

        public override async Task<IEnumerable<TResult>> GetAllAsync<TResult>(CancellationToken cancellationToken)
        {
            var cacheResult = await GetAllCacheAsync<TResult>(cancellationToken);
            if (cacheResult.Value != null && cacheResult.Value.Any())
            {
                return cacheResult.Value;
            }

            var cmd = $@"SELECT * FROM {_tableName} as T 
                         WHERE 
                            T.TenantId = {_token.Context.TenantId}
                            AND T.OwnerId = '{_token.Context.OwnerId}'
                            AND T.IsDeleted = 0
                         ORDER BY T.Name";
            var result = await _dbConnection.QueryAsync<TResult>(cmd);
            if (result.Any())
            {
                await _sequenceCaching.SetAsync(cacheResult.Key, result, TimeSpan.FromDays(7), cancellationToken: cancellationToken);
            }
            return result;
        }

        public async Task<int> GetNextOrderAsync(long categoryId, CancellationToken cancellationToken)
        {
            var cmd = $@"SELECT COALESCE(MAX(`ORDER`), 0) + 1 FROM note n 
                         INNER JOIN note_category nc ON n.CategoryId = nc.Id 
                         WHERE nc.Id = {categoryId} AND n.TenantId = {_token.Context.TenantId} AND nc.OwnerId = {_token.Context.OwnerId}  AND n.IsDeleted = 0;";
            return await _dbConnection.QuerySingleOrDefaultAsync<int>(cmd);
        }
    }
}

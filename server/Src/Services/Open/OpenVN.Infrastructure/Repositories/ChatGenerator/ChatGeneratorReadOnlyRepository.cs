using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using SharedKernel.Auth;
using SharedKernel.Caching;
using SharedKernel.Libraries;
using SharedKernel.MySQL;
using SharedKernel.Runtime.Exceptions;

namespace OpenVN.Infrastructure
{
    public class ChatGeneratorReadOnlyRepository : BaseReadOnlyRepository<ChatGenerator>, IChatGeneratorReadOnlyRepository
    {
        public ChatGeneratorReadOnlyRepository(IDbConnection dbConnection, IToken token, ISequenceCaching sequenceCaching, IServiceProvider provider) : base(dbConnection, token, sequenceCaching, provider)
        {
        }

        public override async Task<TResult> GetByIdAsync<TResult>(object id, CancellationToken cancellationToken)
        {
            var cacheResult = await GetByIdCacheAsync<TResult>(id, cancellationToken);
            if (cacheResult.Value != null)
            {
                return cacheResult.Value;
            }

            var cmd = $"SELECT T.Id, T.FileName, T.CreatedDate, T.Contents FROM {_tableName} as T WHERE T.Id = @Id AND T.OwnerId = '{_token.Context.OwnerId}' AND T.TenantId = '{_token.Context.TenantId}' AND T.IsDeleted = 0";
            var entity = await _dbConnection.QuerySingleOrDefaultAsync<ChatGenerator>(cmd, new { Id = id });

            if (entity != null)
            {
                var result = new ChatGeneratorDto
                {
                    Id = entity.Id.ToString(),
                    FileName = entity.FileName,
                    CreatedDate = entity.CreatedDate,
                    Content = JsonConvert.DeserializeObject<ChatContentDto>(entity.Contents)
                };
                await _sequenceCaching.SetAsync(cacheResult.Key, result, TimeSpan.FromDays(7), cancellationToken: cancellationToken);

                return (TResult)Convert.ChangeType(result, typeof(TResult));
            }
            return default(TResult);
        }

        public override async Task<PagingResult<TResult>> GetPagingAsync<TResult>(PagingRequest request, CancellationToken cancellationToken)
        {
            var cmd = @$"SELECT T.Id, T.FileName, T.CreatedDate FROM {_tableName} as T 
                         WHERE T.OwnerId = '{_token.Context.OwnerId}' AND 
                               T.TenantId = '{_token.Context.TenantId}' AND 
                               T.IsDeleted = 0";
            var countCmd = @$"SELECT Count(Id) FROM {_tableName} as T WHERE T.OwnerId = '{_token.Context.OwnerId}' AND T.TenantId = '{_token.Context.TenantId}' AND T.IsDeleted = 0";

            // Order by
            var limit = $"LIMIT {request.Offset}, {request.Size}";
            if (request.Sorts != null && request.Sorts.Any())
            {

                var tmp = new List<string>();
                foreach (var sort in request.Sorts)
                {
                    if (Secure.DetectSqlInjection(sort.FieldName))
                    {
                        throw new SqlInjectionException();
                    }
                    tmp.Add($" T.{sort.FieldName} {(sort.SortAscending ? "ASC" : "DESC")} ");
                }
                var order = $" ORDER BY {string.Join(",", tmp)} ";
                cmd += $" {order} {limit}";
            }
            else
            {
                cmd += $" ORDER BY CASE WHEN T.LastModifiedDate > T.CreatedDate THEN T.LastModifiedDate ELSE T.CreatedDate END DESC {limit}";
            }
            var dataTask = _dbConnection.QueryAsync<TResult>(cmd, null);
            var countTask = _dbConnection.QuerySingleOrDefaultAsync<long>(countCmd, null);
            
            await Task.WhenAll(dataTask, countTask);
            return new PagingResult<TResult>
            {
                Data = await dataTask,
                Count = await countTask
            };
        }
    }
}

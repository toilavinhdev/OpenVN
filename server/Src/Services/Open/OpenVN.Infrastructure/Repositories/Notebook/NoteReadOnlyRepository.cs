using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Auth;
using SharedKernel.Caching;
using SharedKernel.Libraries;
using SharedKernel.MySQL;
using SharedKernel.Runtime.Exceptions;

namespace OpenVN.Infrastructure
{
    public class NoteReadOnlyRepository : BaseReadOnlyRepository<Note>, INoteReadOnlyRepository
    {
        public NoteReadOnlyRepository(
            IDbConnection dbConnection,
            IToken token,
            ISequenceCaching sequenceCaching,
            IServiceProvider provider
        ) : base(dbConnection, token, sequenceCaching, provider)
        {
        }

        public async Task<List<NoteWithoutContentDto>> SearchAsync(string query, CancellationToken cancellationToken)
        {
            var cacheKey = BaseCacheKeys.GetFullRecordsKey(_tableName, _token.Context.TenantId, _token.Context.OwnerId);
            if (string.IsNullOrEmpty(query))
            {
                var cacheData = await _sequenceCaching.GetAsync<List<NoteWithoutContentDto>>(cacheKey, CachingType.Memory, cancellationToken: cancellationToken);
                if (cacheData != null && cacheData.Any())
                {
                    return cacheData;
                }
            }

            var cmd = $@"SELECT T.*, T2.Name as CategoryName FROM {_tableName} as T 
                        INNER JOIN {new NoteCategory().GetTableName()} AS T2 ON T.CategoryId = T2.Id
                        WHERE T.OwnerId = '{_token.Context.OwnerId}' AND T2.OwnerId = '{_token.Context.OwnerId}' 
                            AND (T.Title LIKE CONCAT('%', @Query, '%') OR T.Content LIKE CONCAT('%', @Query, '%'))
                            AND T.IsDeleted = 0
                            AND T2.IsDeleted = 0
                        ORDER BY T.Order
                       ";
            var result = (await _dbConnection.QueryAsync<NoteWithoutContentDto>(cmd, new { Query = query == null ? string.Empty : query })).ToList();
            if (string.IsNullOrEmpty(query) && result.Any())
            {
                await _sequenceCaching.SetAsync(cacheKey, result, TimeSpan.FromDays(7), onlyUseType: CachingType.Memory, cancellationToken: cancellationToken);
            }
            return result;
        }

        public override async Task<PagingResult<TResult>> GetPagingAsync<TResult>(PagingRequest request, CancellationToken cancellationToken)
        {
            var categoryTableName = new NoteCategory().GetTableName();
            var cmd = $"SELECT T.*, T2.Name AS CategoryName FROM {_tableName} as T INNER JOIN {categoryTableName} T2 ON T.CategoryId = T2.Id WHERE T2.OwnerId = {_token.Context.OwnerId} AND T.IsDeleted = 0 AND T2.IsDeleted = 0";
            var countCmd = $"SELECT Count(T.Id) FROM {_tableName} as T INNER JOIN {categoryTableName} T2 ON T.CategoryId = T2.Id WHERE T2.OwnerId = '{_token.Context.OwnerId}' AND T.IsDeleted = 0 AND T2.IsDeleted = 0";

            // Filter
            var param = new Dictionary<string, object>();
            if (request.Filter != null && request.Filter.Fields != null && request.Filter.Fields.Any())
            {
                var formula = request.Filter.Formula;
                for (int i = 0; i < request.Filter.Fields.Count; i++)
                {
                    var field = request.Filter.Fields[i];
                    var property = typeof(Note).GetProperty(field.FieldName);
                    if (property == null || !Attribute.IsDefined(property, typeof(FilterableAttribute)))
                    {
                        throw new BadRequestException("The filter is invalid");
                    }

                    var hasUnicode = field.Value.HasUnicode();
                    var replaceValue = $" T.{field.FieldName} {field.GetOperatorWithValue(out string paramName, hasUnicode)}";
                    formula = formula.Replace("{" + i + "}", replaceValue);
                    param[paramName] = field.Value;
                }
                cmd += $" AND ({formula}) ";
                countCmd += $" AND ({formula}) ";
            }

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
            var dataTask = _dbConnection.QueryAsync<TResult>(cmd, param);
            var countTask = _dbConnection.QuerySingleOrDefaultAsync<long>(countCmd, param);

            await Task.WhenAll(dataTask, countTask);
            return new PagingResult<TResult>
            {
                Data = await dataTask,
                Count = await countTask
            };
        }

        public override async Task<TResult> GetByIdAsync<TResult>(object id, CancellationToken cancellationToken)
        {
            var cmd = $@"SELECT T.*, T2.Name AS CategoryName 
                         FROM {_tableName} as T 
                         INNER JOIN {new NoteCategory().GetTableName()} T2 ON T.CategoryId = T2.Id 
                         WHERE T.Id = @Id 
                               AND CASE WHEN T.IsPublic = 1 THEN TRUE ELSE T2.OwnerId = '{_token.Context.OwnerId}' END
                               AND T.IsDeleted = 0 AND T2.IsDeleted = 0";
            return await _dbConnection.QuerySingleOrDefaultAsync<TResult>(cmd, new { Id = id });
        }
    }
}

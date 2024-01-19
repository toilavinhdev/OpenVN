using AutoMapper;
using SharedKernel.Auth;
using SharedKernel.Caching;
using SharedKernel.Libraries;
using SharedKernel.MySQL;
using SharedKernel.Runtime.Exceptions;

namespace OpenVN.Infrastructure
{
    public class SpendingReadOnlyRepository : BaseReadOnlyRepository<Spending>, ISpendingReadOnlyRepository
    {
        private readonly IMapper _mapper;

        public SpendingReadOnlyRepository(
            IDbConnection dbConnection,
            IToken token,
            ISequenceCaching sequenceCaching,
            IServiceProvider provider,
            IMapper mapper
        ) : base(dbConnection, token, sequenceCaching, provider)
        {
            _mapper = mapper;
        }

        public override async Task<PagingResult<TResult>> GetPagingAsync<TResult>(PagingRequest request, CancellationToken cancellationToken)
        {
            // Filter
            var param = new Dictionary<string, object>();
            var filter = " AND 1=1 ";

            if (request.Filter != null && request.Filter.Fields != null && request.Filter.Fields.Any())
            {
                var formula = request.Filter.Formula;
                for (int i = 0; i < request.Filter.Fields.Count; i++)
                {
                    var field = request.Filter.Fields[i];
                    var property = typeof(Spending).GetProperty(field.FieldName);
                    if (property == null || !Attribute.IsDefined(property, typeof(FilterableAttribute)))
                    {
                        throw new BadRequestException("The filter is invalid");
                    }

                    var hasUnicode = field.Value.HasUnicode();
                    var replaceValue = $" T.{field.FieldName} {field.GetOperatorWithValue(out string paramName, hasUnicode)}";
                    formula = formula.Replace("{" + i + "}", replaceValue);
                    param[paramName] = field.Value;
                }
                filter += $" AND ({formula}) ";
            }
            param["OwnerId"] = _token.Context.OwnerId;

            var cmd = $@" SELECT (SELECT SUM(s.Value) + T.Value FROM spending s WHERE s.Path LIKE CONCAT(T.Id, '%')) AS Total, T.*, T2.Name AS CategoryName FROM {new Spending().GetTableName()} AS T 
                          INNER JOIN {new SpendingCategory().GetTableName()} T2 ON T.CategoryId = T2.Id
                          WHERE T.OwnerId = @OwnerId AND T.IsDeleted = 0 AND ParentId = 0 AND T2.IsDeleted = 0 {filter} 
                          ORDER BY CASE WHEN T.LastModifiedDate > T.CreatedDate THEN T.LastModifiedDate ELSE T.CreatedDate END DESC 
                          LIMIT {request.Offset}, {request.Size}";

            var countCmd = $@"SELECT Count(Id) FROM {new Spending().GetTableName()} AS T WHERE T.OwnerId = @OwnerId AND T.IsDeleted = 0 AND ParentId = 0 {filter}";
            var dataTask = _dbConnection.QueryAsync<TResult>(cmd, param);
            var countTask = _dbConnection.QuerySingleOrDefaultAsync<long>(countCmd, param);

            await Task.WhenAll(dataTask, countTask);
            return new PagingResult<TResult>
            {
                Data = dataTask.Result,
                Count = countTask.Result
            };
        }

        public async Task<PagingResult<SpendingDto>> GetPagingWithSubAsync(PagingRequest request, CancellationToken cancellationToken)
        {
            // Filter
            var param = new Dictionary<string, object>();
            var filter = string.Empty;

            if (request.Filter != null && request.Filter.Fields != null && request.Filter.Fields.Any())
            {
                var formula = request.Filter.Formula;
                for (int i = 0; i < request.Filter.Fields.Count; i++)
                {
                    var field = request.Filter.Fields[i];
                    var property = typeof(Spending).GetProperty(field.FieldName);
                    if (property == null || !Attribute.IsDefined(property, typeof(FilterableAttribute)))
                    {
                        throw new BadRequestException("The filter is invalid");
                    }

                    var hasUnicode = field.Value.HasUnicode();
                    var replaceValue = $" T.{field.FieldName} {field.GetOperatorWithValue(out string paramName, hasUnicode)}";
                    formula = formula.Replace("{" + i + "}", replaceValue);
                    param[paramName] = field.Value;
                }
                filter += $" AND ({formula}) ";
            }
            param["OwnerId"] = _token.Context.OwnerId;

            var cmd = $@" SELECT T.*, T2.Name AS CategoryName FROM {new Spending().GetTableName()} AS T 
                          INNER JOIN {new SpendingCategory().GetTableName()} T2 ON T.CategoryId = T2.Id
                          WHERE T.OwnerId = @OwnerId AND T.IsDeleted = 0 AND T2.IsDeleted = 0 {filter} 
                          ORDER BY CASE WHEN T.LastModifiedDate > T.CreatedDate THEN T.LastModifiedDate ELSE T.CreatedDate END DESC";

            var countCmd = $@"SELECT Count(Id) FROM {new Spending().GetTableName()} AS T WHERE T.OwnerId = @OwnerId AND T.IsDeleted = 0 AND ParentId = 0 {filter}";
            var dataTask = _dbConnection.QueryAsync<SpendingDto>(cmd, param);
            var countTask = _dbConnection.QuerySingleOrDefaultAsync<long>(countCmd, param);

            await Task.WhenAll(dataTask, countTask);
            return new PagingResult<SpendingDto>
            {
                Data = IndependentsToTree(dataTask.Result.ToList(), "0").Skip(request.Offset).Take(request.Size),
                Count = await countTask
            };
        }

        private List<SpendingDto> IndependentsToTree(List<SpendingDto> input, string parentId)
        {
            var items = input.Where(x => x.ParentId == parentId.ToString()).ToList();
            if (items.Any())
            {
                foreach (var item in items)
                {
                    item.Subs = item.Subs ?? new List<SpendingDto>();
                    item.Subs.AddRange(IndependentsToTree(input, item.Id));
                }
            }
            return items;
        }
    }
}

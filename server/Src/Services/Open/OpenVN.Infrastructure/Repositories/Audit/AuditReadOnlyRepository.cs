using SharedKernel.Auth;
using SharedKernel.Caching;
using SharedKernel.MySQL;

namespace OpenVN.Infrastructure
{
    public class AuditReadOnlyRepository : IAuditReadOnlyRepository
    {
        protected readonly string _tableName = "";
        protected readonly IDbConnection _dbConnection;
        protected readonly IToken _token;
        protected readonly ISequenceCaching _sequenceCaching;

        public AuditReadOnlyRepository(IDbConnection dbConnection, IToken token, ISequenceCaching sequenceCaching)
        {
            _dbConnection = dbConnection;
            _token = token;
            _sequenceCaching = sequenceCaching;
            _tableName = new AuditEntity().GetTableName();
        }

        public async Task<PagingResult<AuditDto>> GetPagingAsync(PagingRequest request, CancellationToken cancellationToken = default)
        {
            var whereClause = $"WHERE T.TenantId = {_token.Context.TenantId} AND T.CreatedBy = {_token.Context.OwnerId}";
            var cmd = $"SELECT Id, IpAddress, TableName, Action, Description, DATE_ADD(Timestamp, INTERVAL 7 hour) as Timestamp FROM {_tableName} as T {whereClause} ORDER BY T.Timestamp DESC LIMIT {request.Offset}, {request.Size}";
            var cmd2 = $"SELECT Count(Id) FROM {_tableName} as T {whereClause}";
            var dataTask = _dbConnection.QueryAsync<AuditDto>(cmd);
            var countTask = _dbConnection.QuerySingleOrDefaultAsync<long>(cmd2);

            await Task.WhenAll(dataTask, countTask);
            return new PagingResult<AuditDto>
            {
                Data = await dataTask,
                Count = await countTask
            };
        }
    }
}

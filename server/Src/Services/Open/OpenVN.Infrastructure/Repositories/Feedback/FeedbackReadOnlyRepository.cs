using SharedKernel.Auth;
using SharedKernel.Caching;
using SharedKernel.MySQL;

namespace OpenVN.Infrastructure
{
    public class FeedbackReadOnlyRepository : BaseReadOnlyRepository<Feedback>, IFeedbackReadOnlyRepository
    {
        public FeedbackReadOnlyRepository(
            IDbConnection dbConnection,
            IToken token,
            ISequenceCaching sequenceCaching,
            IServiceProvider provider
        ) : base(dbConnection, token, sequenceCaching, provider)
        {
        }

        public override async Task<TResult> GetByIdAsync<TResult>(object id, CancellationToken cancellationToken)
        {
            var query = @$"SELECT T.Id, T.OwnerId, T.Content, T.CreatedDate, T.FromIP, T.TenantId, ca.FileName as AvatarUrl, cu.Email
                           FROM {new Feedback().GetTableName()} T  
                           INNER JOIN common_user cu ON T.OwnerId = cu.Id
                           LEFT JOIN common_avatar ca ON (T.OwnerId = ca.OwnerId AND ca.IsDeleted = 0)
                           WHERE T.Id = @Id AND T.IsDeleted = 0 AND T.ParentId = 0";
            return await _dbConnection.QuerySingleOrDefaultAsync<TResult>(query, new { Id = id });
        }

        public override async Task<PagingResult<TResult>> GetPagingAsync<TResult>(PagingRequest request, CancellationToken cancellationToken)
        {
            var query = @$"SELECT T.Id, T.Content, T.CreatedDate, T.FromIP, (SELECT COUNT(*) FROM {new Feedback().GetTableName()} WHERE CONCAT('/', PATH, '/') LIKE CONCAT('%/', T.Id, '/%')) AS ReplyCount 
                           FROM {new Feedback().GetTableName()} T 
                           WHERE T.IsDeleted = 0 AND T.ParentId = 0
                           ORDER BY T.CreatedDate
                           LIMIT {request.Offset}, {request.Size}";
            var queryCount = @$"SELECT COUNT(*) FROM {new Feedback().GetTableName()} WHERE IsDeleted = 0 AND ParentId = 0";
            var result = await _dbConnection.QueryMultipleAsync($"{query};{queryCount}");

            var data = await result.ReadAsync<TResult>();
            var count = await result.ReadSingleAsync<int>();

            return new PagingResult<TResult>
            {
                Data = data,
                Count = count
            };
        }

        public override async Task<IEnumerable<TResult>> GetAllAsync<TResult>(CancellationToken cancellationToken)
        {
            var query = @$"SELECT T.Id, T.OwnerId, T.Content, T.CreatedDate, T.FromIP, T.TenantId, ca.FileName as AvatarUrl, cu.Email
                           FROM {new Feedback().GetTableName()} T  
                           INNER JOIN common_user cu ON T.OwnerId = cu.Id
                           LEFT JOIN common_avatar ca ON (T.OwnerId = ca.OwnerId AND ca.IsDeleted = 0)
                           WHERE T.IsDeleted = 0 AND T.ParentId = 0
                           ORDER BY T.CreatedDate";
            return await _dbConnection.QueryAsync<TResult>(query);
        }

        public async Task<IEnumerable<TResult>> GetRepliesAsync<TResult>(object sourceFeedbackId, CancellationToken cancellationToken)
        {
            var query = @$"SELECT T.Id, T.Content, T.CreatedDate, T.FromIP, (SELECT COUNT(*) FROM {new Feedback().GetTableName()} WHERE CONCAT('/', PATH, '/') LIKE CONCAT('%/', T.Id, '/%')) AS ReplyCount 
                           FROM {new Feedback().GetTableName()} T 
                           WHERE T.IsDeleted = 0 AND T.ParentId = @ParentId
                           ORDER BY T.CreatedDate";
            return await _dbConnection.QueryAsync<TResult>(query, new { ParentId = sourceFeedbackId });
        }

        public async Task<int> GetRepliesCountAsync(object sourceFeedbackId, CancellationToken cancellationToken)
        {
            var query = $"SELECT COUNT(*) FROM {new Feedback().GetTableName()} WHERE CONCAT('/', PATH, '/') LIKE CONCAT('%/', @Id, '/%') AND IsDeleted = 0";
            return await _dbConnection.QuerySingleOrDefaultAsync<int>(query, new { Id = sourceFeedbackId });
        }
    }
}

using SharedKernel.Auth;
using SharedKernel.Caching;
using SharedKernel.Domain;
using SharedKernel.Infrastructures;
using SharedKernel.MySQL;

namespace SharedKernel.Application
{
    public class NotificationReadOnlyRepository : BaseReadOnlyRepository<Notification>, INotificationReadOnlyRepository
    {
        public NotificationReadOnlyRepository(
            IDbConnection dbConnection,
            IToken token,
            ISequenceCaching sequenceCaching,
            IServiceProvider provider
        ) : base(dbConnection, token, sequenceCaching, provider)
        {
            _dbConnection = new DbConnection("CentralizedNotificationsDb");
        }

        public async Task<int> GetNumberOfUnreadNotificationAsync(CancellationToken cancellationToken)
        {
            var cmd = $"SELECT COUNT(*) FROM {new Notification().GetTableName()} WHERE TenantId = {_token.Context.TenantId} AND OwnerId = {_token.Context.OwnerId} AND IsUnread = 1 AND IsDeleted = 0";
            return await _dbConnection.QuerySingleOrDefaultAsync<int>(cmd, cancellationToken);
        }
    }
}

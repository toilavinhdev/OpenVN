using IdGen;
using Microsoft.Extensions.Localization;
using SharedKernel.Auth;
using SharedKernel.Caching;
using SharedKernel.Domain;
using SharedKernel.Infrastructures;
using SharedKernel.Libraries;
using SharedKernel.MySQL;
using SharedKernel.Properties;

namespace SharedKernel.Application
{
    public class NotificationWriteOnlyRepository : BaseWriteOnlyRepository<Notification>, INotificationWriteOnlyRepository
    {
        public NotificationWriteOnlyRepository(
            IDbConnection dbConnection,
            IToken token,
            ISequenceCaching sequenceCaching,
            IStringLocalizer<Resources> localizer
        ) : base(dbConnection, token, sequenceCaching, localizer)
        {
            _dbConnection = new DbConnection("CentralizedNotificationsDb");
        }

        public async Task MarkAsReadOrUnreadAsync(object id, bool markAsRead, CancellationToken cancellationToken)
        {
            var cmd = @$"UPDATE {_tableName} SET IsUnread = {(markAsRead ? "0" : "1")}, LastModifiedDate = @LastModifiedDate, LastModifiedBy = {_token.Context.OwnerId}
                         WHERE Id = @Id AND TenantId = {_token.Context.TenantId} AND OwnerId = {_token.Context.OwnerId} AND IsDeleted = 0";
            await _dbConnection.ExecuteAsync(cmd, new { Id = id, LastModifiedDate = DateHelper.Now });
        }

        public async Task MarkAllAsReadAsync(CancellationToken cancellationToken)
        {
            var cmd = @$"UPDATE {_tableName} SET IsUnread = 0, LastModifiedDate = @LastModifiedDate, LastModifiedBy = {_token.Context.OwnerId}
                         WHERE TenantId = {_token.Context.TenantId} AND OwnerId = {_token.Context.OwnerId} AND IsDeleted = 0";
            await _dbConnection.ExecuteAsync(cmd, new { LastModifiedDate = DateHelper.Now });
        }
    }
}

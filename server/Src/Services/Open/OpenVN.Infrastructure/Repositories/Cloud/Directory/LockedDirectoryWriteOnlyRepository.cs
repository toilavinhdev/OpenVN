using Microsoft.Extensions.Localization;
using SharedKernel.Auth;
using SharedKernel.Caching;
using SharedKernel.Libraries;
using SharedKernel.MySQL;
using SharedKernel.Properties;

namespace OpenVN.Infrastructure
{
    public class LockedDirectoryWriteOnlyRepository : BaseWriteOnlyRepository<LockedDirectory>, ILockedDirectoryWriteOnlyRepository
    {
        public LockedDirectoryWriteOnlyRepository(
            IDbConnection dbConnection,
            IToken token,
            ISequenceCaching sequenceCaching,
            IStringLocalizer<Resources> localizer
        ) : base(dbConnection, token, sequenceCaching, localizer)
        {
        }

        public async Task ChangeLockStatusAsync(long directoryId, string type, CancellationToken cancellationToken)
        {
            var cmd = $@"UPDATE {new LockedDirectory().GetTableName()} 
                            SET EnabledLock = {(type == "lock" ? 1 : 0)},
                                LastModifiedDate = CURRENT_TIMESTAMP(),
                                LastModifiedBy = {_token.Context.OwnerId}
                         WHERE TenantId = {_token.Context.TenantId}
                            AND OwnerId = {_token.Context.OwnerId}
                            AND DirectoryId = {directoryId}
                            AND IsDeleted = 0";

            await _dbConnection.ExecuteAsync(cmd, null);
        }

        public async Task SetPasswordAsync(long directoryId, string password, CancellationToken cancellationToken)
        {
            var sp = JsonHelper.GetConfiguration("other-stored-procedure.json")["SetDirectoryPassword"];
            var param = new Dictionary<string, object>()
            {
                { "v_tenant_id", _token.Context.TenantId },
                { "v_owner_id", _token.Context.OwnerId },
                { "v_directory_id", directoryId },
                { "v_password", password },
            };
            await _dbConnection.ExecuteAsync(sp, param, commandType: System.Data.CommandType.StoredProcedure);
        }
    }
}

using SharedKernel.Auth;
using SharedKernel.Caching;
using SharedKernel.MySQL;
using Directory = OpenVN.Domain.Directory;

namespace OpenVN.Infrastructure
{
    public class LockedDirectoryReadOnlyRepository : BaseReadOnlyRepository<Directory>, ILockedDirectoryReadOnlyRepository
    {
        public LockedDirectoryReadOnlyRepository(IDbConnection dbConnection, IToken token, ISequenceCaching sequenceCaching, IServiceProvider provider) : base(dbConnection, token, sequenceCaching, provider)
        {
        }

        public async Task<bool> CheckPasswordAsync(long directoryId, string password, CancellationToken cancellationToken)
        {
            var cmd = $@"SELECT Id FROM {new LockedDirectory().GetTableName()}
                         WHERE TenantId = {_token.Context.TenantId}
                            AND OwnerId = {_token.Context.OwnerId}
                            AND DirectoryId = {directoryId}
                            AND Password = @Password
                            AND IsDeleted = 0";

            return (await _dbConnection.QueryFirstOrDefaultAsync<LockedDirectory>(cmd, new { Password = password })) != null;
        }
    }
}

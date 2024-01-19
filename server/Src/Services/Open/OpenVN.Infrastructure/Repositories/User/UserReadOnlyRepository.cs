using SharedKernel.Auth;
using SharedKernel.Caching;
using SharedKernel.Libraries;
using SharedKernel.MySQL;

namespace OpenVN.Infrastructure
{
    public class UserReadOnlyRepository : BaseReadOnlyRepository<User>, IUserReadOnlyRepository
    {
        public UserReadOnlyRepository(
            IDbConnection dbConnection,
            IToken token,
            ISequenceCaching sequenceCaching,
            IServiceProvider provider
        ) : base(dbConnection, token, sequenceCaching, provider)
        {
        }

        public async Task<string> CheckDuplicateAsync(string username, string email, string phone, long tenantId, CancellationToken cancellationToken = default)
        {
            var param = new
            {
                v_username = username,
                v_email = email,
                v_phone = phone,
                v_tenant_id = tenantId
            };
            return await _dbConnection.QuerySingleOrDefaultAsync<string>(
                         JsonHelper.GetConfiguration("user-stored-procedure.json")["CheckDuplicateUser"],
                         param,
                         commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task<Avatar> GetAvatarAsync(CancellationToken cancellationToken)
        {
            var query = $"SELECT * FROM {new Avatar().GetTableName()} WHERE TenantId = {_token.Context.TenantId} AND OwnerId = {_token.Context.OwnerId} AND IsDeleted = 0";
            return await _dbConnection.QuerySingleOrDefaultAsync<Avatar>(query);
        }

        public async Task<User> GetUserInformationAsync(CancellationToken cancellationToken)
        {
            var cmd = $"SELECT * FROM {new User().GetTableName()} WHERE Id = {_token.Context.OwnerId} AND TenantId = {_token.Context.TenantId} AND IsDeleted = 0";
            return await _dbConnection.QuerySingleOrDefaultAsync<User>(cmd);
        }
    }
}

using OpenVN.Master.Application;
using OpenVN.Master.Domain;
using SharedKernel.Auth;
using SharedKernel.Caching;
using SharedKernel.Infrastructures;
using SharedKernel.MySQL;

namespace OpenVN.Master.Infrastructure
{
    public class AppReadOnlyRepository : BaseReadOnlyRepository<App>, IAppReadOnlyRepository
    {
        public AppReadOnlyRepository(IDbConnection dbConnection, IToken token, ISequenceCaching sequenceCaching, IServiceProvider provider) : base(dbConnection, token, sequenceCaching, provider)
        {
        }

        public async Task<IEnumerable<AppDto>> GetAppsAsync(CancellationToken cancellationToken)
        {
            var param = new
            {
                v_tenant_id = _token.Context.TenantId,
                v_owner_id = _token.Context.OwnerId,
            };
            return await _dbConnection.QueryAsync<AppDto>("proc_get_apps", param, commandType: System.Data.CommandType.StoredProcedure);
        }

        public override async Task<TResult> GetByIdAsync<TResult>(object id, CancellationToken cancellationToken)
        {
            var cmd = $"SELECT * FROM {new App().GetTableName()} WHERE Id = @Id AND IsDeleted = 0";
            return await _dbConnection.QuerySingleOrDefaultAsync<TResult>(cmd, new { Id = id });
        }
    }
}

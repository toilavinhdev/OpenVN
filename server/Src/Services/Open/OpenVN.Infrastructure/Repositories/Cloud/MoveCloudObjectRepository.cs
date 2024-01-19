using OpenVN.Application;
using SharedKernel.Auth;
using SharedKernel.Libraries;
using SharedKernel.MySQL;
using SharedKernel.UnitOfWork;

namespace OpenVN.Infrastructure
{
    public class MoveCloudObjectRepository : IMoveCloudObjectRepository
    {
        private readonly IDbConnection _dbConnection;
        private readonly IToken _token;

        public MoveCloudObjectRepository(
            IDbConnection dbConnection,
            IToken token
        )
        {
            _dbConnection = dbConnection;
            _token = token;
        }

        public IUnitOfWork UnitOfWork => _dbConnection;

        public async Task MoveObjectAsync(object destinationId, List<MoveObject> moveObjects, CancellationToken cancellationToken)
        {
            var sp = JsonHelper.GetConfiguration("other-stored-procedure.json")["MoveCloudObjects"];
            var param = new Dictionary<string, object>()
            {
                { "v_tenant_id", _token.Context.TenantId },
                { "v_owner_id", _token.Context.OwnerId },
                { "v_destination_id", destinationId },
                { "v_dir_ids", string.Join(",", moveObjects.Where(x => x.Type.Equals("dir")).Select(x => x.SourceId)) },
                { "v_file_ids", string.Join(",", moveObjects.Where(x => x.Type.Equals("cf")).Select(x => x.SourceId)) },
            };

            await _dbConnection.ExecuteAsync(sp, param, commandType: System.Data.CommandType.StoredProcedure);
        }
    }
}

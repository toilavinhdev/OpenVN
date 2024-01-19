using Microsoft.Extensions.Localization;
using SharedKernel.Auth;
using SharedKernel.Caching;
using SharedKernel.Libraries;
using SharedKernel.MySQL;
using SharedKernel.Properties;
using SharedKernel.Runtime.Exceptions;
using Directory = OpenVN.Domain.Directory;

namespace OpenVN.Infrastructure
{
    public class DirectoryWriteOnlyRepository : BaseWriteOnlyRepository<Directory>, IDirectoryWriteOnlyRepository
    {
        private readonly IEventDispatcher _eventDispatcher;

        public DirectoryWriteOnlyRepository(
            IDbConnection dbConnection,
            IToken token,
            ISequenceCaching sequenceCaching,
            IStringLocalizer<Resources> localizer,
            IEventDispatcher eventDispatcher
        ) : base(dbConnection, token, sequenceCaching, localizer)
        {
            _eventDispatcher = eventDispatcher;
        }

        public override async Task<List<Directory>> DeleteAsync(List<string> ids, CancellationToken cancellationToken)
        {
            var joinedIds = string.Join(",", ids);
            if (Secure.DetectSqlInjection(joinedIds))
            {
                throw new SqlInjectionException();
            }

            var sp = JsonHelper.GetConfiguration("other-stored-procedure.json")["GetAllNodesInDirectories"];
            var sp2 = JsonHelper.GetConfiguration("other-stored-procedure.json")["DeleteDirectoriesByIds"];
            var param = new Dictionary<string, object>();
            param["v_tenant_id"] = _token.Context.TenantId;
            param["v_owner_id"] = _token.Context.OwnerId;
            param["v_ids"] = joinedIds;

            var entities = await _dbConnection.QueryAsync<Directory>(sp, param, commandType: System.Data.CommandType.StoredProcedure);
            if (entities.Any())
            {
                await _dbConnection.ExecuteAsync(sp2, param, commandType: System.Data.CommandType.StoredProcedure);
                await ClearCacheWhenChangesAsync(entities.Select(x => (object)x.Id).ToList(), cancellationToken);
            }
            return entities.ToList();
        }

        protected override void BeforeUpdate(Directory entity, Directory oldValue)
        {
            entity.TenantId = _token.Context.TenantId;
            entity.CreatedBy = _token.Context.OwnerId;
            entity.LastModifiedDate = DateHelper.Now;
            entity.LastModifiedBy = _token.Context.OwnerId;
            entity.OwnerId = _token.Context.OwnerId;

            var newValue = (Directory)entity.Clone();
            newValue.ClearDomainEvents();
            newValue.Name = newValue.Name.ToBase64Decode();

            oldValue.ClearDomainEvents();
            oldValue.Name = oldValue.Name.ToBase64Decode();

            entity.AddDomainEvent(new UpdateAuditEvent<Directory>(new List<UpdateAuditModel<Directory>> { new UpdateAuditModel<Directory>(newValue, oldValue) }, _token));
        }
    }
}

using Microsoft.Extensions.Localization;
using SharedKernel.Auth;
using SharedKernel.Caching;
using SharedKernel.MySQL;
using SharedKernel.Properties;

namespace OpenVN.Infrastructure
{
    public class CloudFileWriteOnlyRepository : BaseWriteOnlyRepository<CloudFile>, ICloudFileWriteOnlyRepository
    {
        public CloudFileWriteOnlyRepository(IDbConnection dbConnection, IToken token, ISequenceCaching sequenceCaching, IStringLocalizer<Resources> localizer) : base(dbConnection, token, sequenceCaching, localizer)
        {
        }

        protected override void BeforeDelete(IEnumerable<CloudFile> entities)
        {
            foreach (var entity in entities)
            {
                var clone = (CloudFile)entity.Clone();
                var @event = new DeleteFileAuditEvent(
                    new List<DeleteFileAuditModel>
                    {
                        new DeleteFileAuditModel(clone, null)
                    },
                    _token
                );

                clone.ClearDomainEvents();
                entity.AddDomainEvent(@event);
            }
        }
    }
}

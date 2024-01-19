using SharedKernel.Auth;

namespace OpenVN.Domain
{
    public class DeleteDirectoryAuditEvent : DeleteAuditEvent<Directory>
    {
        public List<DeleteDirectoryAuditModel> Models { get; }

        public DeleteDirectoryAuditEvent(List<DeleteDirectoryAuditModel> models, IToken token) : base(models.Select(x => x.Directory).ToList(), token)
        {
            Models = models;
        }
    }
}

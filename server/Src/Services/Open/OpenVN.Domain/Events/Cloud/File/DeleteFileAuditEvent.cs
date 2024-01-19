using SharedKernel.Auth;

namespace OpenVN.Domain
{
    public class DeleteFileAuditEvent : DeleteAuditEvent<CloudFile>
    {
        public List<DeleteFileAuditModel> Models { get; }

        public DeleteFileAuditEvent(List<DeleteFileAuditModel> models, IToken token) : base(models.Select(x => x.File).ToList(), token)
        {
            Models = models;
        }
    }
}

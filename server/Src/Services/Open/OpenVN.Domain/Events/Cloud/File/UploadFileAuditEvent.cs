using SharedKernel.Auth;

namespace OpenVN.Domain
{
    public class UploadFileAuditEvent : AuditEvent
    {
        public List<UploadFileAuditModel> Models { get; }
        public UploadFileAuditEvent(List<UploadFileAuditModel> models, IToken token, Guid eventId = default) : base(typeof(CloudFile).Name, AuditAction.Upload, token, eventId)
        {
            Models = models;
        }
    }
}

using SharedKernel.Auth;

namespace OpenVN.Domain
{
    public class MoveFileAuditEvent : AuditEvent
    {
        public List<MoveFileAuditModel> Models { get; }

        public MoveFileAuditEvent(List<MoveFileAuditModel> models, IToken token, Guid eventId = default) : base(typeof(CloudFile).Name, AuditAction.Move, token, eventId)
        {
            Models = models;
        }
    }
}

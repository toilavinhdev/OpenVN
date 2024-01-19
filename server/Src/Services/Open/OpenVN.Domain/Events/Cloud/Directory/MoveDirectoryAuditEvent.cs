using SharedKernel.Auth;

namespace OpenVN.Domain
{
    public class MoveDirectoryAuditEvent : AuditEvent
    {
        public List<MoveDirectoryAuditModel> Models { get; }

        public MoveDirectoryAuditEvent( List<MoveDirectoryAuditModel> models, IToken token, Guid eventId = default) : base(typeof(Directory).Name, AuditAction.Move, token, eventId)
        {
            Models = models;
        }
    }
}

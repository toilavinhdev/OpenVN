using SharedKernel.Auth;

namespace SharedKernel.Domain
{
    public class UpdateAvatarAuditEvent : AuditEvent
    {
        public bool IsRemoved { get; }

        public UpdateAvatarAuditEvent(IToken token, bool isRemoved = false, Guid eventId = default) : base("Avatar", AuditAction.Avatar, token, eventId)
        {
            IsRemoved = isRemoved;
        }
    }
}

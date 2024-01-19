using SharedKernel.Auth;

namespace OpenVN.Domain
{
    public class SignOutAuditEvent : AuditEvent
    {
        public SignOutAuditEvent(IToken token, Guid eventId = default) : base("SignOut", AuditAction.SignOut, token, eventId)
        {
        }
    }
}

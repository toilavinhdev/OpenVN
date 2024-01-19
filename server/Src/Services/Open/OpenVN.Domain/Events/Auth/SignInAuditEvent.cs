using SharedKernel.Auth;

namespace OpenVN.Domain
{
    public class SignInAuditEvent : AuditEvent
    {
        public SignInAuditEvent(IToken token, Guid eventId = default) : base("SignIn", AuditAction.SignIn, token, eventId)
        {
        }
    }
}

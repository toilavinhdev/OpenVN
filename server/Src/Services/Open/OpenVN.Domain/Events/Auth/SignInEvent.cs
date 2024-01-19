using SharedKernel.Auth;

namespace OpenVN.Domain
{
    public class SignInEvent : DomainEvent
    {
        public SignInEvent(IToken token, Guid eventId, object body) : base(eventId, body, token)
        {
        }
    }
}

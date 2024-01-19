using SharedKernel.Domain;

namespace OpenVN.BackgroundJob
{
    public class IntegrationSignInEvent : IntegrationEvent
    {
        public IntegrationSignInEvent(string eventId, DateTime timestamp, string eventType, object body) : base(eventId, timestamp, eventType, body)
        {
        }
    }
}

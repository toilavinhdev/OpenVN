using SharedKernel.Domain;

namespace OpenVN.Audit.Events
{
    public class IntegrationUpdateAvatarAuditEvent : IntegrationAuditEvent<BaseEntity>
    {
        public bool IsRemoved { get; set; }
    }
}
